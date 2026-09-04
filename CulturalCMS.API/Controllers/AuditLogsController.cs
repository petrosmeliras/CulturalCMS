using CulturalCMS.Application.DTO;
using CulturalCMS.Domain.Constants;
using CulturalCMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CulturalCMS.API.Controllers
{
    [ApiController]
    [Route("api/v1/cultural-items/{itemId:int}/audit-logs")]
    public class AuditLogsController : ControllerBase   
    {
        private readonly IApplicationService _applicationService;

        public AuditLogsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Retrieves the audit history for a specific cultural item.
        /// </summary>
        /// <param name="itemId">The ID of the cultural item.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
        /// <returns>A list of audit logs representing the item's timeline.</returns>
        /// <response code="200">Returns the audit log timeline for the item.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not a Curator or Admin.</response>
        [HttpGet]
        [Authorize(Roles = AppRoles.CuratorOrAdmin)] 
        [ProducesResponseType(typeof(IEnumerable<AuditLogReadOnlyDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<AuditLogReadOnlyDTO>>> GetItemHistory(int itemId, CancellationToken cancellationToken = default)
        {
            var logs = await _applicationService.AuditLogService.GetLogsByEntityAsync("CulturalItem", itemId, cancellationToken);

            return Ok(logs);
        }
    }
}
