using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.DTO
{
    public record MetadataDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, ErrorMessage = "Key must not exceed 100 characters.")]
        public string? Key { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(500, ErrorMessage = "Value must not exceed 500 characters.")]
        public string? Value { get; set; }
    }
}
