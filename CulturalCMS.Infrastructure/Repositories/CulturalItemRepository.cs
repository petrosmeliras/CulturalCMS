using CulturalCMS.Application.Common;
using CulturalCMS.Application.Interfaces;
using CulturalCMS.Application.SearchQueries;
using CulturalCMS.Domain.Entities;
using CulturalCMS.Domain.Enums;
using CulturalCMS.Infrastructure.Data;
using CulturalCMS.Infrastructure.QueryBuilders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Infrastructure.Repositories
{
    public class CulturalItemRepository : BaseRepository<CulturalItem>, ICulturalItemRepository
    {

        public CulturalItemRepository(CulturalDbContext context)
            : base(context)
        {
        }

        public override async Task<IEnumerable<CulturalItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.Metadata)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CulturalItem>> GetByOwnerIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.Metadata)
                .Where(c => c.CreatedById == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var existingEntity = await _dbSet.FindAsync(id);
            if (existingEntity is null) return false;

            existingEntity.IsDeleted = true;
            existingEntity.DeletedAt = DateTime.UtcNow;
            _context.Entry(existingEntity).State = EntityState.Modified;

            return true;
        }
        public async Task<CulturalItem?> GetItemWithMetadataAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.Metadata)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<PaginatedResult<CulturalItem>> SearchAsync(ItemSearchQuery query, CancellationToken cancellationToken = default)
        {
            var dbQuery = _dbSet
                .Include(c => c.Metadata)
                .AsQueryable();

            //if (!string.IsNullOrWhiteSpace(query.Status))
            //{
            //    if (Enum.TryParse<ItemStatus>(query.Status, true, out var parsedStatus))
            //    {
            //        dbQuery = dbQuery.Where(c => c.Status == parsedStatus);
            //    }
            //}

            //if (!string.IsNullOrWhiteSpace(query.Category))
            //{
            //    var category = query.Category.ToLower();
            //    dbQuery = dbQuery.Where(c => c.Category.ToLower().Contains(category));
            //}

            //if (!string.IsNullOrWhiteSpace(query.HistoricalPeriod))
            //{
            //    var historicalPeriod = query.HistoricalPeriod.ToLower();
            //    dbQuery = dbQuery.Where(c => c.HistoricalPeriod.ToLower().Contains(historicalPeriod));
            //}

            //if (query.MetadataFilters != null && query.MetadataFilters.Any())
            //{
            //    foreach (var filter in query.MetadataFilters)
            //    {
            //        dbQuery = dbQuery.Where(c => c.Metadata.Any(m => 
            //        m.Key.ToLower() == filter.Key.ToLower() && m.Value.ToLower() == filter.Value.ToLower()));
            //    }

            //}

            //if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            //{
            //    var term = query.SearchTerm.ToLower();
            //    dbQuery = dbQuery.Where(c =>
            //    c.Title.ToLower().Contains(term) ||
            //    c.Description.ToLower().Contains(term) ||
            //    c.Category.ToLower().Contains(term) ||
            //    c.HistoricalPeriod.ToLower().Contains(term) ||
            //    c.Metadata.Any(m => m.Key.ToLower().Contains(term) || m.Value.ToLower().Contains(term))
            //    );
            //}

            dbQuery = CulturalItemQueryBuilder.ApplyFilters(dbQuery, query);

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = CulturalItemQueryBuilder.ApplySorting(dbQuery, query.SortBy, query.SortOrder);

            //var sortBy = query.SortBy?.ToLower();
            //var isDescending = query.SortOrder?.ToLower() == "desc";

            //if (sortBy == "title")
            //{
            //    dbQuery = isDescending
            //        ? dbQuery.OrderByDescending(c => c.Title)
            //        : dbQuery.OrderBy(c => c.Title);
            //}
            //else if (sortBy == "createdat")
            //{
            //    dbQuery = isDescending
            //        ? dbQuery.OrderByDescending(c => c.CreatedAt)
            //        : dbQuery.OrderBy(c => c.CreatedAt);
            //}
            //else if (sortBy == "viewcount" || sortBy == "popularity")
            //{
            //    dbQuery = isDescending
            //        ? dbQuery.OrderByDescending(c => c.ViewCount)
            //        : dbQuery.OrderBy(c => c.ViewCount);
            //}
            //else
            //{
            //    dbQuery = isDescending
            //        ? dbQuery.OrderByDescending(c => c.Id)
            //        : dbQuery.OrderBy(c => c.Id);
            //}

            var items = await dbQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<CulturalItem>(items, totalCount, query.PageNumber, query.PageSize);


        }
    }
}
