using CulturalCMS.Application.BusinessServices;
using CulturalCMS.Application.DTO;
using CulturalCMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CulturalCMS.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public AuthController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Registers a new user (as Contributor) in the system.
        /// </summary>
        /// <param name="signupDTO">The registration details of the new user.</param>
        /// <returns>The created user details.</returns>
        /// <response code="201">Returns the newly created user.</response>
        /// <response code="400">If the registration data is invalid or the username already exists.</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [EnableRateLimiting("StrictPolicy")]
        [ProducesResponseType(typeof(UserReadOnlyDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserReadOnlyDTO>> Register([FromBody] UserSignupDTO signupDTO)
        {
            var createdUser = await _applicationService.AuthService.RegisterUserAsync(signupDTO);

            return CreatedAtAction(
                actionName: nameof(UsersController.GetUserById),
                controllerName: "Users",
                routeValues: new { id = createdUser.Id },
                value: createdUser);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary> 
        /// <param name="credentials">The user's login credentials.</param>
        /// <returns>A JWT token if the authentication is successful.</returns>
        /// <response code="200">Returns the JWT token.</response>
        /// <response code="401">If the username or password is incorrect.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("StrictPolicy")]
        [ProducesResponseType(typeof(JwtTokenDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<JwtTokenDTO>> Login([FromBody] UserLoginDTO credentials)
        {
            var user = await _applicationService.UserService.VerifyAndGetUserAsync(credentials);

            var token = _applicationService.UserService.CreateUserToken(user);

            return Ok(new JwtTokenDTO(token));
        }
    }
}
