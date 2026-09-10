namespace WorkoutTracker.Application.Common.Interfaces;

public interface ITokenService
{
    TimeSpan RefreshTokenLifetime { get; }
    string IssueAccessToken(Guid userId, string email);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}
