using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.DTO
{
    public record UserLoginDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 50 characters.")]
        public string Username { get; init; } = string.Empty;

        [Required(ErrorMessage = "The {0} field is required.")]
        public string Password { get; init; } = string.Empty;
    }
}
