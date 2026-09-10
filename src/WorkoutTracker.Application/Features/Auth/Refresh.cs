using WorkoutTracker.Application.Common.Exceptions;
using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Domain.Auth;
using WorkoutTracker.Domain.Common;

namespace WorkoutTracker.Application.Features.Auth;

public sealed record RefreshCommand(string RefreshToken);

public static class RefreshHandler
{
    private const string InvalidRefreshToken = "Invalid or expired refresh token";

    public static async Task<AuthResult> Handle(
        RefreshCommand command,
        IIdentityService identityService,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.RefreshToken))
        {
            throw new AuthenticationFailedException(InvalidRefreshToken);
        }

        var now = timeProvider.GetUtcNow();
        var tokenHash = tokenService.HashRefreshToken(command.RefreshToken);
        var refreshToken = await refreshTokenRepository.GetByHashAsync(tokenHash, cancellationToken);

        if (refreshToken is null || !refreshToken.IsActive(now))
        {
            throw new AuthenticationFailedException(InvalidRefreshToken);
        }

        var user = await identityService.FindByIdAsync(refreshToken.UserId, cancellationToken);

        if (user is null)
        {
            throw new AuthenticationFailedException(InvalidRefreshToken);
        }

        refreshToken.Revoke(now);

        return await AuthTokens.IssueAsync(
            user,
            tokenService,
            refreshTokenRepository,
            unitOfWork,
            timeProvider,
            cancellationToken);
    }
}
