using AutoMapper;
using CulturalCMS.Application.Common;
using CulturalCMS.Application.DTO;
using CulturalCMS.Application.Interfaces;
using CulturalCMS.Application.SearchQueries;
using CulturalCMS.Domain.Constants;
using CulturalCMS.Domain.Entities;
using CulturalCMS.Domain.Enums;
using CulturalCMS.Domain.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;

namespace CulturalCMS.Application.BusinessServices
{
    public class CulturalItemService : ICulturalItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CulturalItemService> _logger;
        private readonly IMemoryCache _cache;

        private const string CacheKeyPublished = "CulturalItemsCache_Published";
        private const string CacheKeyAll = "CulturalItemsCache_All";

        public CulturalItemService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CulturalItemService> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        private void InvalidateCache()
        {
            _cache.Remove(CacheKeyPublished);
            _cache.Remove(CacheKeyAll);
        }

        private static string GetItemSnapshot(CulturalItem item)
        {
            return JsonSerializer.Serialize(new
            {
                item.Title,
                item.Description,
                item.Category,
                item.HistoricalPeriod,
                Metadata = item.Metadata.Select(m => new { m.Key, m.Value }).ToList()
            });
        }

        private async Task LogAuditAsync(int userId, AuditAction action, int entityId,
            string? oldValues, string? newValues, string? changedColumns = null)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityName = "CulturalItem",
                EntityId = entityId,
                OldValues = oldValues,
                NewValues = newValues,
                ChangedColumns = changedColumns,
                Timestamp = DateTime.UtcNow
            };
            await _unitOfWork.AuditLogRepository.AddAsync(auditLog);
        }

        public async Task<IEnumerable<CulturalItemReadOnlyDTO>> GetAllItemsAsync(CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.CulturalItemRepository.GetAllAsync(cancellationToken);
            var orderedItems = items.OrderBy(x => x.Id).ToList();
            _logger.LogInformation("Retrieved {Count} cultural items", items.Count());
            return _mapper.Map<IEnumerable<CulturalItemReadOnlyDTO>>(orderedItems);
        }

        public async Task<IEnumerable<CulturalItemReadOnlyDTO>> GetPublishedItemsAsync(CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.CulturalItemRepository.GetAllAsync(cancellationToken);
            var publishedItems = items
                .Where(x => x.Status == ItemStatus.Published)
                .OrderBy(x => x.Id)
                .ToList();

            _logger.LogInformation("Retrieved {Count} published cultural items", publishedItems.Count);
            return _mapper.Map<IEnumerable<CulturalItemReadOnlyDTO>>(publishedItems);
        }

        public async Task<CulturalItemReadOnlyDTO> GetItemByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _unitOfWork.CulturalItemRepository.GetItemWithMetadataAsync(id, cancellationToken);
            if (item == null)
            {
                throw new EntityNotFoundException("CulturalItem", $"Cultural item with id {id} not found");
            }

            await _unitOfWork.CulturalItemRepository.ViewCountAsync(id, cancellationToken);
            item.ViewCount++;
            
            _logger.LogInformation("Cultural item with id {id} found", id);
            return _mapper.Map<CulturalItemReadOnlyDTO>(item);
        }

        public async Task<CulturalItemReadOnlyDTO> CreateItemAsync(CulturalItemCreateDTO createDTO, int userId)
        {
            var item = _mapper.Map<CulturalItem>(createDTO);

            item.CreatedById = userId;
            item.Status = ItemStatus.Draft;

            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

            await _unitOfWork.CulturalItemRepository.AddAsync(item);
            await _unitOfWork.SaveAsync();

            await LogAuditAsync(userId, AuditAction.Create, item.Id, oldValues: null, newValues: GetItemSnapshot(item));
            await _unitOfWork.SaveAsync();

            scope.Complete();

            InvalidateCache();

            _logger.LogInformation("Cultural item created with ID {Id}", item.Id);
            return _mapper.Map<CulturalItemReadOnlyDTO>(item);
        }

        public async Task<PaginatedResult<CulturalItemReadOnlyDTO>> SearchItemsAsync(ItemSearchQuery query, bool isPrivileged, CancellationToken cancellationToken = default)
        {
            var effectiveQuery = isPrivileged
                ? query
                : query with { Status = ItemStatus.Published.ToString() };

            bool isDefaultSearch = string.IsNullOrEmpty(query.SearchTerm)
                && string.IsNullOrEmpty(query.Category)
                && string.IsNullOrEmpty(query.HistoricalPeriod)
                && string.IsNullOrEmpty(query.Status)
                && string.IsNullOrEmpty(query.MetadataKey)
                && string.IsNullOrEmpty(query.MetadataValue)
                && query.PageNumber == 1
                && string.Equals(query.SortBy, "CreatedAt", StringComparison.OrdinalIgnoreCase)
                && string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

            var cacheKey = isPrivileged ? CacheKeyAll : CacheKeyPublished;

            if (isDefaultSearch && _cache.TryGetValue(cacheKey, out PaginatedResult<CulturalItemReadOnlyDTO>? cachedResult))
            {
                _logger.LogInformation("Cultural items returned from Cache.");
                return cachedResult!;

            }

            var result = await _unitOfWork.CulturalItemRepository.SearchAsync(effectiveQuery, null, cancellationToken);
            var dtos = _mapper.Map<List<CulturalItemReadOnlyDTO>>(result.Data);

            var paginatedResponse = new PaginatedResult<CulturalItemReadOnlyDTO>
            {
                Data = dtos,
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };

            if (isDefaultSearch)
            {
                _cache.Set(cacheKey, paginatedResponse, TimeSpan.FromMinutes(10));
            }

            return paginatedResponse;
        }

        public async Task<PaginatedResult<CulturalItemReadOnlyDTO>> SearchMyItemsAsync(
           ItemSearchQuery query, int userId, CancellationToken cancellationToken = default)
        {
            var result = await _unitOfWork.CulturalItemRepository.SearchAsync(query, userId, cancellationToken);

            var dtos = _mapper.Map<List<CulturalItemReadOnlyDTO>>(result.Data);

            _logger.LogInformation("Retrieved {Count} cultural items for user {UserId}", dtos.Count, userId);

            return new PaginatedResult<CulturalItemReadOnlyDTO>
            {
                Data = dtos,
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
            };
        }

        public async Task UpdateItemAsync(int id, CulturalItemUpdateDTO updateDTO, int userId, string userRole)
        {
            var item = await _unitOfWork.CulturalItemRepository.GetItemWithMetadataAsync(id);
            if (item == null)
            {
                throw new EntityNotFoundException("CulturalItem", $"Cultural item with id {id} not found");
            }

            if (userRole == AppRoles.Contributor)
            {
                if (item.Status != ItemStatus.Draft)
                {
                    throw new InvalidItemStateException("CulturalItem", "Only items in Draft state can be edited.");
                }

                if (item.CreatedById != userId)
                {
                    throw new EntityForbiddenException("CulturalItem", "You do not have permission to edit items created by other users.");
                }
            }
            else if (userRole != AppRoles.Admin)
            {
                throw new EntityForbiddenException("CulturalItem", "You do not have permission to edit cultural items.");
            }

            var oldValues = GetItemSnapshot(item);

            var oldTitle = item.Title;
            var oldDescription = item.Description;
            var oldCategory = item.Category;
            var oldHistoricalPeriod = item.HistoricalPeriod;
            var oldMetadataJson = JsonSerializer.Serialize(item.Metadata.Select(m => new { m.Key, m.Value }).ToList());

            _mapper.Map(updateDTO, item);

            // Remove existing metadata instead of diffing: clearing and re-adding lets
            // EF Core track the changes more easily, and avoids issues with duplicate keys or missing keys.
            item.Metadata.Clear();
            foreach (var metaDTO in updateDTO.Metadata)
            {
                item.Metadata.Add(new ItemMetadata { Key = metaDTO.Key!, Value = metaDTO.Value! });
            }

            var newValues = GetItemSnapshot(item);

            var changedColumns = new List<string>();
            if (oldTitle != item.Title) changedColumns.Add(nameof(item.Title));
            if (oldDescription != item.Description) changedColumns.Add(nameof(item.Description));
            if (oldCategory != item.Category) changedColumns.Add(nameof(item.Category));
            if (oldHistoricalPeriod != item.HistoricalPeriod) changedColumns.Add(nameof(item.HistoricalPeriod));

            var newMetadataJson = JsonSerializer.Serialize(item.Metadata.Select(m => new { m.Key, m.Value }).ToList());
            if (oldMetadataJson != newMetadataJson) changedColumns.Add(nameof(item.Metadata));

            await _unitOfWork.CulturalItemRepository.UpdateAsync(item);

            await LogAuditAsync(userId, AuditAction.Update, item.Id, oldValues, newValues, string.Join(", ", changedColumns));
            await _unitOfWork.SaveAsync();

            InvalidateCache();

            _logger.LogInformation("Cultural item with id {id} updated", id);
        }

        public async Task DeleteItemAsync(int id, int userId, string userRole)
        {
            var item = await _unitOfWork.CulturalItemRepository.GetByIdAsync(id);

            if (item == null)
            {
                throw new EntityNotFoundException("CulturalItem", $"Cultural item with ID {id} was not found.");
            }

            var oldValues = GetItemSnapshot(item);

            await _unitOfWork.CulturalItemRepository.DeleteAsync(item.Id);

            await LogAuditAsync(userId, AuditAction.Delete, item.Id, oldValues, newValues: null);
            await _unitOfWork.SaveAsync();

            InvalidateCache();

            _logger.LogInformation("Cultural item with id {Id} deleted", id);
        }

        public async Task SubmitItemAsync(int id, int userId, string userRole)
        {
            var item = await _unitOfWork.CulturalItemRepository.GetByIdAsync(id);
            if (item == null)
                throw new EntityNotFoundException("CulturalItem", $"Cultural item with id {id} not found.");

            if (item.Status != ItemStatus.Draft)
                throw new InvalidItemStateException("CulturalItem", "Only items in Draft status can be submitted for review.");

            if (userRole == AppRoles.Contributor && item.CreatedById != userId)
                throw new EntityForbiddenException("CulturalItem", "Contributors can only submit their own items.");

            item.Status = ItemStatus.ForReview;

            await _unitOfWork.CulturalItemRepository.UpdateAsync(item);

            await LogAuditAsync(userId, AuditAction.StatusChange, item.Id,
                oldValues: "{\"Status\": \"Draft\"}",
                newValues: "{\"Status\": \"ForReview\"}");
            await _unitOfWork.SaveAsync();

            InvalidateCache();
        }

        public async Task ApproveItemAsync(int id, int userId, string userRole)
        {
            if (userRole != AppRoles.Curator && userRole != AppRoles.Admin)
                throw new EntityForbiddenException("CulturalItem", "Only Curators and Admins can approve items.");

            var item = await _unitOfWork.CulturalItemRepository.GetByIdAsync(id);
            if (item == null)
                throw new EntityNotFoundException("CulturalItem", $"Cultural item with id {id} not found.");

            if (item.Status != ItemStatus.ForReview)
                throw new InvalidItemStateException("CulturalItem", "Only items in ForReview status can be approved.");

            item.Status = ItemStatus.Published;
            item.PublishedAt = DateTime.UtcNow;

            await _unitOfWork.CulturalItemRepository.UpdateAsync(item);

            await LogAuditAsync(userId, AuditAction.StatusChange, item.Id,
                oldValues: "{\"Status\": \"ForReview\"}",
                newValues: "{\"Status\": \"Published\"}");
            await _unitOfWork.SaveAsync();

            InvalidateCache();
        }
        public async Task RejectItemAsync(int id, int userId, string userRole)
        {
            if (userRole != AppRoles.Curator && userRole != AppRoles.Admin)
                throw new EntityForbiddenException("CulturalItem", "Only Curators and Admins can reject items.");

            var item = await _unitOfWork.CulturalItemRepository.GetByIdAsync(id);
            if (item == null)
                throw new EntityNotFoundException("CulturalItem", $"Cultural item with id {id} not found.");

            if (item.Status != ItemStatus.ForReview)
                throw new InvalidItemStateException("CulturalItem", "Only items in ForReview status can be rejected.");

            item.Status = ItemStatus.Draft;

            await _unitOfWork.CulturalItemRepository.UpdateAsync(item);

            await LogAuditAsync(userId, AuditAction.StatusChange, item.Id,
                oldValues: "{\"Status\": \"ForReview\"}",
                newValues: "{\"Status\": \"Draft\"}");
            await _unitOfWork.SaveAsync();

            InvalidateCache();
        }
    }
}