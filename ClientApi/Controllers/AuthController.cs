using System.Net.Mime;
using ClientApi.Models;
using ClientApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientApi.Controllers;

[Route("api/auth")]
[ApiController]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    /// <summary>Logs in</summary>
    /// <returns>An AuthResponseDto object</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/auth/login
    ///     {
    ///         "login": "admin",
    ///         "password": "admin123"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Returns an access token and etc.</response>
    /// <response code="400">If request is not valid</response>
    /// <response code="401">If unauthorized</response>
    [HttpPost("login")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Login, request.Password);
        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    /// <summary>Logs out</summary>
    /// <returns>An AuthResponseDto object</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/auth/logout
    ///     -H "X-Refresh-Token: my-refresh-token"
    /// 
    /// </remarks>
    /// <response code="204">Log out is successful</response>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Headers["X-Refresh-Token"].ToString();
        await _authService.LogoutAsync(refreshToken);

        return NoContent();
    }

    /// <summary>Refreshs token</summary>
    /// <returns>An AuthResponseDto object</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/auth/refresh
    ///     {
    ///         "refresh_token": "your-refresh-token"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Returns an access token and etc.</response>
    /// <response code="400">RefreshToken is required</response>
    /// <response code="401">If data is invalid</response>
    [HttpPost("refresh")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Refresh(TokenRefreshRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }
}