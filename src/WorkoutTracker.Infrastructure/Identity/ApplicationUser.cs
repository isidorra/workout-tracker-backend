using Microsoft.AspNetCore.Identity;

namespace WorkoutTracker.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
