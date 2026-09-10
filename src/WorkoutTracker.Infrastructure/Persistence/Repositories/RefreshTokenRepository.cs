using Microsoft.EntityFrameworkCore;

using WorkoutTracker.Domain.Auth;

namespace WorkoutTracker.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public Task<RefreshToken> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return context.RefreshTokens.SingleOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);
    }

    public void Add(RefreshToken refreshToken)
    {
        context.RefreshTokens.Add(refreshToken);
    }
}
