using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace WorkoutTracker.Infrastructure.Authentication;

public static class JwtAuthenticationExtensions
{
    private const int MinimumSigningKeyBytes = 32;

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(JwtOptions.SectionName);
        var options = section.Get<JwtOptions>()
            ?? throw new InvalidOperationException($"The '{JwtOptions.SectionName}' configuration section is missing");

        if (string.IsNullOrWhiteSpace(options.Issuer) || string.IsNullOrWhiteSpace(options.Audience))
        {
            throw new InvalidOperationException($"{JwtOptions.SectionName}:Issuer and {JwtOptions.SectionName}:Audience must be configured");
        }

        if (string.IsNullOrWhiteSpace(options.SigningKey) || Encoding.UTF8.GetByteCount(options.SigningKey) < MinimumSigningKeyBytes)
        {
            throw new InvalidOperationException($"{JwtOptions.SectionName}:SigningKey must be at least {MinimumSigningKeyBytes} bytes");
        }

        services.Configure<JwtOptions>(section);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.MapInboundClaims = false;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        return services;
    }
}
