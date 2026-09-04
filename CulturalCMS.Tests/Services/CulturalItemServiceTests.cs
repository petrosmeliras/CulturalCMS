using AutoMapper;
using CulturalCMS.Application.BusinessServices;
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
    // Unit tests for the CulturalItem workflow state machine (Submit / Approve / Reject).
    public class CulturalItemServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CulturalItemService> _logger;
        private readonly IMemoryCache _cache;
        private readonly CulturalItemService _service;

        public CulturalItemServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<CulturalItemService>>();
            _cache = Substitute.For<IMemoryCache>();
            _service = new CulturalItemService(_unitOfWork, _mapper, _logger, _cache);
        }

        private static CulturalItem CreateDummyItem(int id = 1, ItemStatus status = ItemStatus.Draft, int createdById = 10)
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
                ViewCount = 0
            };
        }

        // ===== SubmitItemAsync =====

        [Fact]
        public async Task SubmitItemAsync_WhenItemIsDraftAndOwnedByContributor_ChangesStatusToForReview()
        {
            // Arrange
            var item = CreateDummyItem(status: ItemStatus.Draft, createdById: 5);
            _unitOfWork.CulturalItemRepository.GetByIdAsync(item.Id).Returns(item);

            // Act
            await _service.SubmitItemAsync(item.Id, userId: 5, userRole: "Contributor");

            // Assert
            Assert.Equal(ItemStatus.ForReview, item.Status);
            await _unitOfWork.CulturalItemRepository.Received(1).UpdateAsync(item);
            await _unitOfWork.AuditLogRepository.Received(1).AddAsync(Arg.Any<AuditLog>());
            await _unitOfWork.Received(1).SaveAsync();
        }

        [Fact]
        public async Task SubmitItemAsync_WhenItemNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            _unitOfWork.CulturalItemRepository.GetByIdAsync(99).Returns((CulturalItem?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _service.SubmitItemAsync(99, userId: 5, userRole: "Contributor"));

            await _unitOfWork.CulturalItemRepository.DidNotReceive().UpdateAsync(Arg.Any<CulturalItem>());
            await _unitOfWork.DidNotReceive().SaveAsync();
        }

        [Fact]
        public async Task SubmitItemAsync_WhenItemIsNotDraft_ThrowsInvalidItemStateException()
        {
            // Arrange
            var item = CreateDummyItem(status: ItemStatus.Published, createdById: 5);
            _unitOfWork.CulturalItemRepository.GetByIdAsync(item.Id).Returns(item);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidItemStateException>(
                () => _service.SubmitItemAsync(item.Id, userId: 5, userRole: "Contributor"));

            await _unitOfWork.CulturalItemRepository.DidNotReceive().UpdateAsync(Arg.Any<CulturalItem>());
        }

        [Fact]
        public async Task SubmitItemAsync_WhenContributorIsNotOwner_ThrowsEntityForbiddenException()
        {
            // Arrange: item owned by user 5; user 7 attempts to submit it.
            var item = CreateDummyItem(status: ItemStatus.Draft, createdById: 5);
            _unitOfWork.CulturalItemRepository.GetByIdAsync(item.Id).Returns(item);

            // Act & Assert
            await Assert.ThrowsAsync<EntityForbiddenException>(
                () => _service.SubmitItemAsync(item.Id, userId: 7, userRole: "Contributor"));

            await _unitOfWork.CulturalItemRepository.DidNotReceive().UpdateAsync(Arg.Any<CulturalItem>());
        }

        [Fact]
        public async Task SubmitItemAsync_WhenAdminSubmitsSomeoneElsesDraftItem_Succeeds()
        {
            // Arrange
            var item = CreateDummyItem(status: ItemStatus.Draft, createdById: 5);
            _unitOfWork.CulturalItemRepository.GetByIdAsync(item.Id).Returns(item);

            // Act
            await _service.SubmitItemAsync(item.Id, userId: 99, userRole: "Admin");

            // Assert
            Assert.Equal(ItemStatus.ForReview, item.Status);
        }

        // ===== ApproveItemAsync =====

        [Fact]
        public async Task ApproveItemAsync_WhenItemIsForReviewAndUserIsCurator_ChangesStatusToPublished()
        {
            // Arrange
            var item = CreateDummyItem(status: ItemStatus.ForReview);
            _unitOfWork.CulturalItemRepository.GetByIdAsync(item.Id).Returns(item);

            // Act
            await _service.ApproveItemAsync(item.Id, userId: 20, userRole: "Curator");

            // Assert
            Assert.Equal(ItemStatus.Published, item.Status);
            Assert.NotNull(item.PublishedAt);
            await _unitOfWork.CulturalItemRepository.Received(1).UpdateAsync(item);
        }

        [Fact]
        public async Task ApproveItemAsync_WhenUserIsContributor_ThrowsEntityForbiddenException()
        {
            // Arrange
            var item = CreateDummyItem(status: ItemStatus.ForReview);

            // Act & Assert
            await Assert.ThrowsAsync<EntityForbiddenException>(
                () => _service.ApproveItemAsync(item.Id, userId: 5, userRole: "Contributor"));

            await _unitOfWork.CulturalItemRepository.DidNotReceive().GetByIdAsync(Arg.Any<int>());
        }

        [Fact]
        public async Task ApproveItemAsync_WhenItemIsNotForReview_ThrowsInvalidItemStateException()
        {
            // Arrange
            var item = CreateDummyItem(status: ItemStatus.Draft);
            _unitOfWork.CulturalItemRepository.GetByIdAsync(item.Id).Returns(item);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidItemStateException>(
                () => _service.ApproveItemAsync(item.Id, userId: 20, userRole: "Curator"));

            await _unitOfWork.CulturalItemRepository.DidNotReceive().UpdateAsync(Arg.Any<CulturalItem>());
        }

        // ===== RejectItemAsync =====

        [Fact]
        public async Task RejectItemAsync_WhenItemIsForReviewAndUserIsAdmin_ChangesStatusBackToDraft()
        {
            // Arrange
            var item = CreateDummyItem(status: ItemStatus.ForReview);
            _unitOfWork.CulturalItemRepository.GetByIdAsync(item.Id).Returns(item);

            // Act
            await _service.RejectItemAsync(item.Id, userId: 30, userRole: "Admin");

            // Assert
            Assert.Equal(ItemStatus.Draft, item.Status);
            await _unitOfWork.CulturalItemRepository.Received(1).UpdateAsync(item);
        }

        [Fact]
        public async Task RejectItemAsync_WhenUserIsContributor_ThrowsEntityForbiddenException()
        {
            // Arrange
            var item = CreateDummyItem(status: ItemStatus.ForReview);

            // Act & Assert
            await Assert.ThrowsAsync<EntityForbiddenException>(
                () => _service.RejectItemAsync(item.Id, userId: 5, userRole: "Contributor"));
        }

        [Fact]
        public async Task RejectItemAsync_WhenItemIsNotForReview_ThrowsInvalidItemStateException()
        {
            // Arrange
            var item = CreateDummyItem(status: ItemStatus.Published);
            _unitOfWork.CulturalItemRepository.GetByIdAsync(item.Id).Returns(item);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidItemStateException>(
                () => _service.RejectItemAsync(item.Id, userId: 20, userRole: "Curator"));

            await _unitOfWork.CulturalItemRepository.DidNotReceive().UpdateAsync(Arg.Any<CulturalItem>());
        }
    }
}
