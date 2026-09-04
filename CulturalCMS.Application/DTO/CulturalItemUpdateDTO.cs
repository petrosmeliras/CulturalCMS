using CulturalCMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.DTO
{
    public record CulturalItemUpdateDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(2000, ErrorMessage = "Description must not exceed 2000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, ErrorMessage = "Category must not exceed 100 characters.")]
        public string? Category { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, ErrorMessage = "Historical Period must not exceed 100 characters.")]
        public string? HistoricalPeriod { get; set; }
        public string? ImageUrl { get; set; }
        public List<MetadataDTO> Metadata { get; set; } = new();
    }
}
