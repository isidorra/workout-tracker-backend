namespace WorkoutTracker.Application.Features.Auth;

public sealed record AuthResult(string AccessToken, string RefreshToken, DateTimeOffset RefreshTokenExpiresAt);
