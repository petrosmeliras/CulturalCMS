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

        public async Task<PaginatedResult<CulturalItem>> SearchAsync(ItemSearchQuery query, int? ownerId = null, CancellationToken cancellationToken = default)
        {
            var dbQuery = _dbSet
                .Include(c => c.Metadata)
                .AsQueryable();

            if (ownerId.HasValue)
            {
                dbQuery = dbQuery.Where(c => c.CreatedById == ownerId.Value);
            }

            dbQuery = CulturalItemQueryBuilder.ApplyFilters(dbQuery, query);

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = CulturalItemQueryBuilder.ApplySorting(dbQuery, query.SortBy, query.SortOrder);

            var items = await dbQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<CulturalItem>(items, totalCount, query.PageNumber, query.PageSize);


        }

        public async Task ViewCountAsync(int id, CancellationToken cancellationToken = default)
            => await _dbSet.Where(c => c.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.ViewCount, c => c.ViewCount + 1), cancellationToken);
    }
}
