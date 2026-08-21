using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Domain.Entities
{
    public class ItemMetadata 
    {
        public int Id { get; set; }
        public int CulturalItemId { get; set; }
        public CulturalItem CulturalItem { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
