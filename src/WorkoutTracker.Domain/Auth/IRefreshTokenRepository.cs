namespace WorkoutTracker.Domain.Auth;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    void Add(RefreshToken refreshToken);
}
