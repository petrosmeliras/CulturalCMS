using CulturalCMS.Application.DTO;
using CulturalCMS.Application.Interfaces;
using CulturalCMS.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CulturalCMS.API.Controllers
{
    [ApiController]
    [Route("api/v1/images")]
    public class ImagesController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ImagesController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Uploads an image file to the server and returns the URL of the uploaded image.
        /// </summary>
        /// <param name="uploadDto">The image file to upload.</param>
        /// <returns>The relative URL of the uploaded image.</returns>
        /// <response code="200">Returns the URL of the uploaded image.</response>
        /// <response code="400">If no file was provided, the file exceeds the 5MB size limit, or the file type is not allowed (only jpg, jpeg, png).</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user lacks permission to upload images.</response>
        [HttpPost("upload")]
        [Authorize(Roles = AppRoles.AnyRole)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadDTO uploadDto)
        {
            var imageUrl = await _applicationService.ImageService.UploadImageAsync(uploadDto);
            return Ok(new { ImageUrl = imageUrl });
        }
    }
}
