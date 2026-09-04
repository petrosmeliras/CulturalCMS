using CulturalCMS.Application.Common;
using CulturalCMS.Application.DTO;
using CulturalCMS.Application.DTO.Filters;
using CulturalCMS.Application.Interfaces;
using CulturalCMS.Domain.Constants;
using CulturalCMS.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CulturalCMS.API.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public UsersController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
        /// <returns>The user details.</returns>
        /// <response code="200">Returns the requested user.</response>
        /// <response code="401">If the request is not authenticated</response>
        /// <response code="404">If no user exists with the given ID</response>
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(UserReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserReadOnlyDTO>> GetUserById(int id, CancellationToken cancellationToken = default)
        {
            EnsureCanViewUser(id);
            var user = await _applicationService.UserService.GetUserByIdAsync(id, cancellationToken);
            return Ok(user);
        }

        /// <summary>
        /// Gets a user by their username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
        /// <returns>The user details.</returns>
        /// <response code="200">Returns the requested user.</response>
        /// <response code="401">If the request is not authenticated</response>
        /// <response code="404">If no user exists with the given username</response>
        [HttpGet("by-username/{username}")]
        [Authorize]
        [ProducesResponseType(typeof(UserReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserReadOnlyDTO>> GetUserByUsername(string username, CancellationToken cancellationToken = default)
        {
            EnsureCanViewUser(username);
            var user = await _applicationService.UserService.GetUserByUsernameAsync(username, cancellationToken);
            return Ok(user);
        }

        /// <summary>
        /// Gets a paginated list of users with optional filtering.
        /// </summary>  
        /// <param name="pageNumber">The page number (1-based. Default is 1.</param>
        /// <param name="pageSize">The number of users to include per page. Default is 10.</param>
        /// <param name="filters">Optional filters for username, email and role.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
        /// <returns>A paginated list of users matching the filters</returns>
        /// <response code="200">Returns the paginated user list.</response>
        /// <response code="401">If the request is not authenticated</response>
        /// <response code="403">If the user does not have permission to view the list</response>
        [HttpGet]
        [Authorize(Roles = AppRoles.Admin)]
        [ProducesResponseType(typeof(PaginatedResult<UserReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResult<UserReadOnlyDTO>>> GetUsers(
            [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery, Range(1,100)] int pageSize = 10,
            [FromQuery] UserFiltersDTO? filters = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _applicationService.UserService
                .GetPaginatedUsersFilteredAsync(pageNumber, pageSize, filters ?? new UserFiltersDTO(), cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Updates the role of a specific user. (Admin only)
        /// </summary>
        /// <param name="id">The Id of the user to update.</param>
        /// <param name="dto">The DTO containing the new role name.</param>
        /// <returns>The updated user details.</returns>
        /// <response code="200">Returns the newly updated user.</response>
        /// <response code="401">If the request is not authenticated.</response>
        /// <response code="403">If the user does not have Admin privileges.</response>
        /// <response code="404">If the user or the specified role is not found.</response>
        [HttpPut("{id:int}/role")]
        [Authorize(Roles = AppRoles.Admin)]
        [ProducesResponseType(typeof(UserReadOnlyDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserReadOnlyDTO>> UpdateUserRole(int id, [FromBody] UserRoleUpdateDTO dto)
        {
            var updatedUser = await _applicationService.UserService.UpdateUserRoleAsync(id, dto.RoleName);
            return Ok(updatedUser);
        }

        private void EnsureCanViewUser(int targetUserId)
        {
            var currentUserIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? currentUserId = currentUserIdString != null ? int.Parse(currentUserIdString) : null;
            var isOwnProfile = currentUserId == targetUserId;

            EnsureCanViewUser(isOwnProfile);
        }

        private void EnsureCanViewUser(string username)
        {
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            var isOwnProfile = string.Equals(currentUsername, username, StringComparison.OrdinalIgnoreCase);

            EnsureCanViewUser(isOwnProfile);
        }

        private void EnsureCanViewUser(bool isOwnProfile)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var isAdmin = string.Equals(currentUserRole, AppRoles.Admin, StringComparison.OrdinalIgnoreCase);

            if (!isOwnProfile && !isAdmin)
            {
                throw new EntityForbiddenException("User", "You do not have permission to view this user.");
            }
        }
    }
}
