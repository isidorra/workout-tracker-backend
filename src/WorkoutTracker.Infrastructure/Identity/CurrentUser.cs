using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

using WorkoutTracker.Application.Common.Interfaces;

namespace WorkoutTracker.Infrastructure.Identity;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var subject = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(subject, out var userId))
            {
                throw new InvalidOperationException("The current request has no authenticated user");
            }

            return userId;
        }
    }
}
