using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Domain.Auth;
using WorkoutTracker.Domain.Common;

namespace WorkoutTracker.Application.Features.Auth;

public sealed record LogoutCommand(string RefreshToken);

public static class LogoutHandler
{
    public static async Task Handle(
        LogoutCommand command,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.RefreshToken))
        {
            return;
        }

        var tokenHash = tokenService.HashRefreshToken(command.RefreshToken);
        var refreshToken = await refreshTokenRepository.GetByHashAsync(tokenHash, cancellationToken);

        if (refreshToken is null)
        {
            return;
        }

        refreshToken.Revoke(timeProvider.GetUtcNow());

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
