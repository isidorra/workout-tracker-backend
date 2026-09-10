using WorkoutTracker.Domain.Common;

namespace WorkoutTracker.Domain.Auth;

public sealed class RefreshToken : Entity
{
    private RefreshToken()
    {
    }

    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    public static RefreshToken Create(Guid userId, string tokenHash, DateTimeOffset now, TimeSpan lifetime)
    {
        return new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAt = now,
            ExpiresAt = now.Add(lifetime)
        };
    }

    public bool IsActive(DateTimeOffset now)
    {
        return RevokedAt is null && ExpiresAt > now;
    }

    public void Revoke(DateTimeOffset now)
    {
        if (RevokedAt is not null)
        {
            return;
        }

        RevokedAt = now;
    }
}
