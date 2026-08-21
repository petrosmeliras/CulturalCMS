using CulturalCMS.Domain.Enums;
using CulturalCMS.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Domain.Entities
{
    public class CulturalItem : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;    
        public string Category { get; set; } = null!;   
        public string HistoricalPeriod { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int ViewCount { get; set; }
        public ItemStatus Status { get; set; } 
        public DateTime? PublishedAt { get; set; }
        public int CreatedById { get; set; }
        public User Creator { get; set; } = null!;
        public Dimensions? Dimensions { get; set; }
        public Coordinates? Coordinates { get; set; }
        public ICollection<ItemMetadata> Metadata { get; set; } = new HashSet<ItemMetadata>();
    }
}
