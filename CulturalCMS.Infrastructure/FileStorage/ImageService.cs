using CulturalCMS.Application.DTO;
using CulturalCMS.Application.Interfaces;
using CulturalCMS.Domain.Exceptions;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Infrastructure.FileStorage
{
    public class ImageService : IImageService
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; 

        private readonly IWebHostEnvironment _environment;
        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<string> UploadImageAsync(ImageUploadDTO uploadDTO)
        {
            var file = uploadDTO.File;

            if (file == null || file.Length == 0)
            {
                throw new InvalidArgumentException("Image", "No image uploaded.");
            }

            if (file.Length > MaxFileSizeBytes)
                throw new InvalidArgumentException("Image", "File size exceeds the 5MB limit.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new InvalidArgumentException("Image", "Only image files (jpg, jpeg, png) are allowed.");

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/uploads/{uniqueFileName}";
        }
    }
}
