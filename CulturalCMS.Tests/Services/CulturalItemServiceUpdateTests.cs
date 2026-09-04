using AutoMapper;
using CulturalCMS.Application.BusinessServices;
using CulturalCMS.Application.DTO;
using CulturalCMS.Application.Interfaces;
using CulturalCMS.Domain.Entities;
using CulturalCMS.Domain.Enums;
using CulturalCMS.Domain.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace CulturalCMS.Tests.Services
{
    // Unit tests for the CulturalItem CRUD permission guards (UpdateItemAsync, plus DeleteItemAsync "not found").
    // Ownership/role for deletion is enforced at the controller level, so it is not tested here.
    public class CulturalItemServiceUpdateTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CulturalItemService> _logger;
        private readonly IMemoryCache _cache;
        private readonly CulturalItemService _service;

        public CulturalItemServiceUpdateTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<CulturalItemService>>();
            _cache = Substitute.For<IMemoryCache>();
            _service = new CulturalItemService(_unitOfWork, _mapper, _logger, _cache);
        }

        private static CulturalItem CreateDummyItem(int id = 1, ItemStatus status = ItemStatus.Draft, int createdById = 5)
        {
            return new CulturalItem
            {
                Id = id,
                Title = "Δίσκος της Φαιστού",
                Description = "Πήλινος δίσκος με ιερογλυφική γραφή.",
                Category = "Γλυπτό",
                HistoricalPeriod = "Μινωική Περίοδος",
                Status = status,
                CreatedById = createdById,
                Metadata = new List<ItemMetadata>()
            };
        }

        private static CulturalItemUpdateDTO CreateDummyUpdateDTO()
        {
            return new CulturalItemUpdateDTO
            {
                Title = "Νέος τίτλος",
                Description = "Νέα περιγραφή",
                Category = "Γλυπτό",
                HistoricalPeriod = "Μινωική Περίοδος",
                Metadata = new List<MetadataDTO>()
            };
        }

        [Fact]
        public async Task UpdateItemAsync_WhenItemNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            _unitOfWork.CulturalItemRepository.GetItemWithMetadataAsync(99).Returns((CulturalItem?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _service.UpdateItemAsync(99, CreateDummyUpdateDTO(), userId: 5, userRole: "Contributor"));

            await _unitOfWork.CulturalItemRepository.DidNotReceive().UpdateAsync(Arg.Any<CulturalItem>());
            await _unitOfWork.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task UpdateItemAsync_WhenContributorIsNotOwner_ThrowsEntityForbiddenException()
        {
            // Arrange: item owned by user 5; user 7 attempts to edit it.
            var item = CreateDummyItem(status: ItemStatus.Draft, createdById: 5);
            _unitOfWork.CulturalItemRepository.GetItemWithMetadataAsync(item.Id).Returns(item);

            // Act & Assert
            await Assert.ThrowsAsync<EntityForbiddenException>(
                () => _service.UpdateItemAsync(item.Id, CreateDummyUpdateDTO(), userId: 7, userRole: "Contributor"));

            await _unitOfWork.CulturalItemRepository.DidNotReceive().UpdateAsync(Arg.Any<CulturalItem>());
        }

        [Fact]
        public async Task UpdateItemAsync_WhenContributorEditsNonDraftItem_ThrowsInvalidItemStateException()
        {
            // Arrange: a Contributor may only edit items still in Draft.
            var item = CreateDummyItem(status: ItemStatus.Published, createdById: 5);
            _unitOfWork.CulturalItemRepository.GetItemWithMetadataAsync(item.Id).Returns(item);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidItemStateException>(
                () => _service.UpdateItemAsync(item.Id, CreateDummyUpdateDTO(), userId: 5, userRole: "Contributor"));

            await _unitOfWork.CulturalItemRepository.DidNotReceive().UpdateAsync(Arg.Any<CulturalItem>());
        }

        [Fact]
        public async Task UpdateItemAsync_WhenUserIsCurator_ThrowsEntityForbiddenException()
        {
            // Arrange: Curators approve/reject but do not edit item content.
            var item = CreateDummyItem(status: ItemStatus.Draft, createdById: 5);
            _unitOfWork.CulturalItemRepository.GetItemWithMetadataAsync(item.Id).Returns(item);

            // Act & Assert
            await Assert.ThrowsAsync<EntityForbiddenException>(
                () => _service.UpdateItemAsync(item.Id, CreateDummyUpdateDTO(), userId: 42, userRole: "Curator"));

            await _unitOfWork.CulturalItemRepository.DidNotReceive().UpdateAsync(Arg.Any<CulturalItem>());
        }

        [Fact]
        public async Task DeleteItemAsync_WhenItemNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            _unitOfWork.CulturalItemRepository.GetByIdAsync(99).Returns((CulturalItem?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _service.DeleteItemAsync(99, userId: 1, userRole: "Admin"));

            await _unitOfWork.CulturalItemRepository.DidNotReceive().DeleteAsync(Arg.Any<int>());
            await _unitOfWork.DidNotReceive().SaveAsync();
        }
    }
}
