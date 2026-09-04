using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.DTO
{
    public record UserSignupDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 50 characters.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?\W)^.{8,25}$",
            ErrorMessage = "Password must be between 8 and 25 characters and contain at least one uppercase, one lowercase, " +
            "one digit, and one special character")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Firstname must be between 2 and 50 characters.")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Lastname must be between 2 and 50 characters.")]
        public string? Lastname { get; set; }
    }
}
