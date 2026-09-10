using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Domain.Auth;
using WorkoutTracker.Domain.Common;

namespace WorkoutTracker.Application.Features.Auth;

internal static class AuthTokens
{
    public static async Task<AuthResult> IssueAsync(
        UserIdentity user,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var token = tokenService.GenerateRefreshToken();
        var refreshToken = RefreshToken.Create(
            user.Id,
            tokenService.HashRefreshToken(token),
            timeProvider.GetUtcNow(),
            tokenService.RefreshTokenLifetime);

        refreshTokenRepository.Add(refreshToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResult(
            tokenService.IssueAccessToken(user.Id, user.Email),
            token,
            refreshToken.ExpiresAt);
    }
}
