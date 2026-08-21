using CulturalCMS.Application.SearchQueries;
using CulturalCMS.Domain.Entities;
using CulturalCMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Infrastructure.QueryBuilders
{
    public static class CulturalItemQueryBuilder
    {
        public static IQueryable<CulturalItem> ApplyFilters(IQueryable<CulturalItem> query, ItemSearchQuery searchQuery)
        {
            if (!string.IsNullOrWhiteSpace(searchQuery.Status))
            {
                if (Enum.TryParse<ItemStatus>(searchQuery.Status, true, out var parsedStatus))
                {
                    query = query.Where(c => c.Status == parsedStatus);
                }
            }

            if (!string.IsNullOrWhiteSpace(searchQuery.Category))
            {
                var category = searchQuery.Category.ToLower();
                query = query.Where(c => c.Category.ToLower().Contains(category));
            }

            if (!string.IsNullOrWhiteSpace(searchQuery.HistoricalPeriod))
            {
                var historicalPeriod = searchQuery.HistoricalPeriod.ToLower();
                query = query.Where(c => c.HistoricalPeriod.ToLower().Contains(historicalPeriod));
            }

            if (searchQuery.MetadataFilters != null && searchQuery.MetadataFilters.Any())
            {
                foreach (var filter in searchQuery.MetadataFilters)
                {
                    query = query.Where(c => c.Metadata.Any(m =>
                    m.Key.ToLower() == filter.Key.ToLower() && m.Value.ToLower().Contains(filter.Value.ToLower())));
                }
            }

            if (!string.IsNullOrWhiteSpace(searchQuery.SearchTerm))
            {
                var term = searchQuery.SearchTerm.ToLower();
                query = query.Where(c =>
                    c.Title.ToLower().Contains(term) ||
                    c.Description.ToLower().Contains(term) ||
                    c.Category.ToLower().Contains(term) ||
                    c.HistoricalPeriod.ToLower().Contains(term) ||
                    c.Metadata.Any(m => m.Key.ToLower().Contains(term) || m.Value.ToLower().Contains(term))
                );
            }
            return query;
        }

        public static IQueryable<CulturalItem> ApplySorting(IQueryable<CulturalItem> query, string? sortBy, string? sortOrder)
        {
            var normalizedSortBy = sortBy?.ToLower();
            var isDescending = sortOrder?.ToLower() == "desc";

            if (normalizedSortBy == "title")
            {
                return isDescending ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title);
            }
            if (normalizedSortBy == "createdat")
            {
                return isDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt);
            }
            if (normalizedSortBy == "viewcount" || normalizedSortBy == "popularity")
            {
                return isDescending ? query.OrderByDescending(c => c.ViewCount) : query.OrderBy(c => c.ViewCount);
            }

            return isDescending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id);
        }
    }
}
