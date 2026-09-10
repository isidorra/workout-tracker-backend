namespace WorkoutTracker.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<UserIdentity> RegisterAsync(string name, string email, string password);
    Task<UserIdentity> ValidateCredentialsAsync(string email, string password);
    Task<UserIdentity> FindByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}

public sealed record UserIdentity(Guid Id, string Name, string Email);
