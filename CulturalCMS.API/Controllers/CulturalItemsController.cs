using CulturalCMS.Application.Common;
using CulturalCMS.Application.DTO;
using CulturalCMS.Application.Interfaces;
using CulturalCMS.Application.SearchQueries;
using CulturalCMS.Domain.Constants;
using CulturalCMS.Domain.Enums;
using CulturalCMS.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace CulturalCMS.API.Controllers
{
    [ApiController]
    [Route("api/v1/cultural-items")]
    public class CulturalItemsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly ILogger<CulturalItemsController> _logger;

        public CulturalItemsController(
            IApplicationService applicationService,
            ILogger<CulturalItemsController> logger)
        {
            _applicationService = applicationService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all published cultural items. Public endpoint - no authentication required.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
        /// <returns>A list of published cultural items.</returns>
        /// <response code="200">Returns the list of items.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CulturalItemReadOnlyDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CulturalItemReadOnlyDTO>>> GetAll(CancellationToken cancellationToken)
        {
            var published = await _applicationService.CulturalItemService.GetPublishedItemsAsync(cancellationToken);
            return Ok(published);
        }

        /// <summary>
        /// Retrieves all cultural items regardless of status (Draft, ForReview, Published), for
        /// content management purposes. Restricted to Curators and Admins.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
        /// <returns>A list of all cultural items.</returns>
        /// <response code="200">Returns the list of all items.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not a Curator or Admin.</response>
        [HttpGet("all")]
        [Authorize(Roles = AppRoles.CuratorOrAdmin)]
        [ProducesResponseType(typeof(IEnumerable<CulturalItemReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<CulturalItemReadOnlyDTO>>> GetAllUnfiltered(CancellationToken cancellationToken)
        {
            var all = await _applicationService.CulturalItemService.GetAllItemsAsync(cancellationToken);
            return Ok(all);
        }

        /// <summary>
        /// Searches and filters cultural items (any status) created by the currently
        /// authenticated user, with pagination. Restricted to Contributors and Admins.
        /// </summary>
        /// <param name="query">The search parameters and filters.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
        /// <returns>A paginated result of the user's own cultural items.</returns>
        /// <response code="200">Returns the paginated list of the user's own items.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not a Contributor or Admin.</response>
        [HttpGet("my-items")]
        [Authorize(Roles = AppRoles.ContributorOrAdmin)]
        [EnableRateLimiting("StrictPolicy")]
        [ProducesResponseType(typeof(IEnumerable<CulturalItemReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<CulturalItemReadOnlyDTO>>> SearchMyItems([FromQuery] ItemSearchQuery query, CancellationToken cancellationToken)
        {
            var items = await _applicationService.CulturalItemService.SearchMyItemsAsync(query, GetCurrentUserId(), cancellationToken);
            return Ok(items);
        }

        /// <summary>
        /// Retrieves a specific cultural item by its ID. Non-Published items are visible only
        /// to Curators/Admins, or to the Contributor who created them.
        /// </summary>
        /// <param name="id">The ID of the item.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
        /// <returns>The requested cultural item.</returns>
        /// <response code="200">Returns the requested item.</response>
        /// <response code="404">If the item is not found or not visible to the current user.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CulturalItemReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CulturalItemReadOnlyDTO>> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _applicationService.CulturalItemService.GetItemByIdAsync(id, cancellationToken);

            if (item.Status != nameof(ItemStatus.Published) && !IsPrivilegedUser())
            {
                var isOwner = TryGetCurrentUserId(out int currentUserId) && item.CreatedById == currentUserId;

                if (!isOwner)
                {
                    return NotFound(); 
                }
            }
            return Ok(item);
        }

        /// <summary>
        /// Searches and filters published cultural items with pagination. Public endpoint -
        /// no authentication required. Always returns only items with Published status.
        /// </summary>
        /// <param name="query">The search parameters and filters.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
        /// <returns>A paginated result of cultural items.</returns>
        /// <response code="200">Returns the paginated list of items.</response>
        [HttpGet("search")]
        [EnableRateLimiting("StrictPolicy")]
        [ProducesResponseType(typeof(PaginatedResult<CulturalItemReadOnlyDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] ItemSearchQuery query, CancellationToken cancellationToken)
        {
            var result = await _applicationService.CulturalItemService.SearchItemsAsync(query, isPrivileged: false, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Searches and filters cultural items across all statuses with pagination, for content
        /// management purposes (e.g. an "Items for Review" queue via the Status filter).
        /// Restricted to Curators and Admins.
        /// </summary>
        /// <param name="query">The search parameters and filters.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
        /// <returns>A paginated result of cultural items across all statuses.</returns>
        /// <response code="200">Returns the paginated list of items.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not a Curator or Admin.</response>
        [HttpGet("search/all")]
        [Authorize(Roles = AppRoles.CuratorOrAdmin)]
        [EnableRateLimiting("StrictPolicy")]
        [ProducesResponseType(typeof(PaginatedResult<CulturalItemReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SearchAllStatuses([FromQuery] ItemSearchQuery query, CancellationToken cancellationToken)
        {
            var result = await _applicationService.CulturalItemService.SearchItemsAsync(query, isPrivileged: true, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new cultural item. Default status will be Draft.
        /// </summary>
        /// <param name="createDTO">The details of the item to create.</param>
        /// <returns>The newly created item.</returns>
        /// <response code="201">Returns the newly created item.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user lacks permission.</response>
        [HttpPost]
        [Authorize(Roles = AppRoles.ContributorOrAdmin)]
        [ProducesResponseType(typeof(CulturalItemReadOnlyDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CulturalItemReadOnlyDTO>> Create([FromBody] CulturalItemCreateDTO createDTO)
        {
            var createdItem = await _applicationService.CulturalItemService.CreateItemAsync(createDTO, GetCurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
        }

        /// <summary>
        /// Updates an existing cultural item.
        /// </summary>
        /// <param name="id">The ID of the item to update.</param>
        /// <param name="updateDTO">The updated item details.</param>
        /// <response code="204">If the item was updated successfully.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user lacks permission.</response>
        /// <response code="404">If the item is not found.</response>
        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.ContributorOrAdmin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] CulturalItemUpdateDTO updateDTO)
        {
            await _applicationService.CulturalItemService.UpdateItemAsync(id, updateDTO, GetCurrentUserId(), GetCurrentUserRole());

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific cultural item.
        /// </summary>
        /// <param name="id">The ID of the item to delete.</param>
        /// <response code="204">If the item was deleted successfully.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user lacks permission (Admin required).</response>
        /// <response code="404">If the item is not found.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _applicationService.CulturalItemService.DeleteItemAsync(id, GetCurrentUserId(), GetCurrentUserRole());
            return NoContent();
        }

        /// <summary>
        /// Submits a Draft item for review by a Curator or Admin.
        /// </summary>
        /// <param name="id">The ID of the item to submit.</param>
        /// <returns>No content if the operation succeeds.</returns>
        /// <response code="204">The item was successfully submitted for review.</response>
        /// <response code="400">If the item is not in a valid state to be submitted.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user is not the owner of the item, or lacks the required role.</response>
        /// <response code="404">If no item exists with the given ID.</response>
        [HttpPost("{id:int}/submit")]
        [Authorize(Roles = AppRoles.ContributorOrAdmin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Submit(int id)
        {
            await _applicationService.CulturalItemService.SubmitItemAsync(id, GetCurrentUserId(), GetCurrentUserRole());
            return NoContent();
        }

        /// <summary>
        /// Approves a ForReview item and marks it as Published.
        /// </summary>
        /// <param name="id">The ID of the item to approve.</param>
        /// <returns>No content if the operation succeeds.</returns>
        /// <response code="204">The item was successfully approved and published.</response>
        /// <response code="400">If the item is not in a valid state to be approved.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks the required role.</response>
        /// <response code="404">If no item exists with the given ID.</response>
        [HttpPost("{id:int}/approve")]
        [Authorize(Roles = AppRoles.CuratorOrAdmin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Approve(int id)
        {          
            await _applicationService.CulturalItemService.ApproveItemAsync(id, GetCurrentUserId(), GetCurrentUserRole());
            return NoContent();
        }

        /// <summary>
        /// Rejects a ForReview item and returns it to Draft status.
        /// </summary>
        /// <param name="id">The ID of the item to reject.</param>
        /// <returns>No content if the operation succeeds.</returns>
        /// <response code="204">The item was successfully rejected and reverted to Draft.</response>
        /// <response code="400">If the item is not in a valid state to be rejected.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user lacks the required role.</response>
        /// <response code="404">If no item exists with the given ID.</response>
        [HttpPost("{id:int}/reject")]
        [Authorize(Roles = AppRoles.CuratorOrAdmin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reject(int id)
        {          
            await _applicationService.CulturalItemService.RejectItemAsync(id, GetCurrentUserId(), GetCurrentUserRole());
            return NoContent();
        }

        private bool IsPrivilegedUser()
        {
            return User.Identity?.IsAuthenticated == true
                   && (User.IsInRole(AppRoles.Admin) || User.IsInRole(AppRoles.Curator));
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdString, out userId);
        }

        private int GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                throw new EntityNotAuthorizedException("User", "ID claim is missing or invalid.");
            }
            return userId;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value
                ?? throw new EntityNotAuthorizedException("User", "Role claim is missing.");
        }
    }
}
