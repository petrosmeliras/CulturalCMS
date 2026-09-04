using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.SearchQueries
{
    public record ItemSearchQuery
    {
        public string? SearchTerm { get; init; }

        public string? Category { get; init; }
        public string? HistoricalPeriod { get; init; }
        public string? Status { get; init; }
        public string? MetadataKey { get; init; }
        public string? MetadataValue { get; init; }
        public string? SortBy { get; init; } = "CreatedAt";
        public string? SortOrder { get; init; } = "desc";

        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1.")]
        public int PageNumber { get; init; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; init; } = 10;
    }
}
