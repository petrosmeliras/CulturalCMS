using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CulturalCMS.Application.DTO
{
    public record ImageUploadDTO
    {
        [Required(ErrorMessage = "Please select a file to upload.")]
        public IFormFile File { get; init; } = null!;
    }
}
