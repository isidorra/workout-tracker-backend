using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Wolverine;

using WorkoutTracker.Api.Contracts;
using WorkoutTracker.Application.Features.Auth;

namespace WorkoutTracker.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IMessageBus bus, IHostEnvironment environment) : ControllerBase
{
    private const string RefreshTokenCookie = "refresh_token";
    private const string RefreshTokenCookiePath = "/api/auth";

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AccessTokenResponse>> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<AuthResult>(command, cancellationToken);

        return Authenticated(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AccessTokenResponse>> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<AuthResult>(command, cancellationToken);

        return Authenticated(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<AccessTokenResponse>> Refresh(CancellationToken cancellationToken)
    {
        var command = new RefreshCommand(Request.Cookies[RefreshTokenCookie]);
        var result = await bus.InvokeAsync<AuthResult>(command, cancellationToken);

        return Authenticated(result);
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await bus.InvokeAsync(new LogoutCommand(Request.Cookies[RefreshTokenCookie]), cancellationToken);

        Response.Cookies.Delete(RefreshTokenCookie, RefreshTokenCookieOptions());

        return NoContent();
    }

    [HttpGet("me")]
    public async Task<ActionResult<MeResponse>> Me(CancellationToken cancellationToken)
    {
        var me = await bus.InvokeAsync<MeResponse>(new GetMeQuery(), cancellationToken);

        return Ok(me);
    }

    private ActionResult<AccessTokenResponse> Authenticated(AuthResult result)
    {
        var options = RefreshTokenCookieOptions();
        options.Expires = result.RefreshTokenExpiresAt;

        Response.Cookies.Append(RefreshTokenCookie, result.RefreshToken, options);

        return Ok(new AccessTokenResponse(result.AccessToken));
    }

    private CookieOptions RefreshTokenCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            // Local development runs over plain HTTP, where not every browser accepts Secure cookies.
            Secure = !environment.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Path = RefreshTokenCookiePath
        };
    }
}
