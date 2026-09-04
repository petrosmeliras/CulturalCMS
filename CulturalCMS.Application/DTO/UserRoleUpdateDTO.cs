using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.DTO
{
    public record UserRoleUpdateDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        public string RoleName { get; init; } = string.Empty;
    }
}
