using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Application.Features.Auth;
using WorkoutTracker.Domain.Auth;
using WorkoutTracker.Domain.Common;
using WorkoutTracker.Infrastructure.Authentication;
using WorkoutTracker.Infrastructure.Identity;
using WorkoutTracker.Infrastructure.Persistence;
using WorkoutTracker.Infrastructure.Persistence.Repositories;

namespace WorkoutTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings:Postgres is not configured");

        services.AddDbContext<AppDbContext>(
            options => options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention(),
            optionsLifetime: ServiceLifetime.Singleton);

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                // The user name is the email, which is validated on its own.
                options.User.AllowedUserNameCharacters = string.Empty;

                options.Password.RequiredLength = PasswordPolicy.MinimumLength;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddJwtAuthentication(configuration);
        services.AddHttpContextAccessor();
        services.TryAddSingleton(TimeProvider.System);

        services.AddSingleton<ITokenService, TokenService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
