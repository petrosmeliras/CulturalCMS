using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.DTO
{
    public record CulturalItemReadOnlyDTO
    {
        public int Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public string HistoricalPeriod { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public int ViewCount { get; init; }
        public int CreatedById { get; init; }
        public string? ImageUrl { get; init; } 
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public DateTime? PublishedAt { get; init; }
        public List<MetadataDTO> Metadata { get; init; } = new();
    }
}
