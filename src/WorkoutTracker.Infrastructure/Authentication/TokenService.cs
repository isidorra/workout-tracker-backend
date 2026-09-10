using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using WorkoutTracker.Application.Common.Interfaces;

namespace WorkoutTracker.Infrastructure.Authentication;

public sealed class TokenService(IOptions<JwtOptions> options, TimeProvider timeProvider) : ITokenService
{
    private const int RefreshTokenBytes = 64;

    private readonly JwtOptions _options = options.Value;
    private readonly JsonWebTokenHandler _tokenHandler = new();

    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(_options.RefreshTokenDays);

    public string IssueAccessToken(Guid userId, string email)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddMinutes(_options.AccessTokenMinutes),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                [JwtRegisteredClaimNames.Sub] = userId.ToString(),
                [JwtRegisteredClaimNames.Email] = email,
                [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString()
            }
        };

        return _tokenHandler.CreateToken(descriptor);
    }

    public string GenerateRefreshToken()
    {
        return Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(RefreshTokenBytes));
    }

    public string HashRefreshToken(string refreshToken)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }
}
