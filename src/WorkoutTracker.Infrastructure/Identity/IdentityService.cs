using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using WorkoutTracker.Application.Common.Exceptions;
using WorkoutTracker.Application.Common.Interfaces;

namespace WorkoutTracker.Infrastructure.Identity;

public sealed class IdentityService(UserManager<ApplicationUser> userManager, TimeProvider timeProvider) : IIdentityService
{
    private static readonly string[] DuplicateErrorCodes =
    [
        nameof(IdentityErrorDescriber.DuplicateEmail),
        nameof(IdentityErrorDescriber.DuplicateUserName)
    ];

    public async Task<UserIdentity> RegisterAsync(string name, string email, string password)
    {
        var user = new ApplicationUser
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            UserName = email,
            Email = email,
            CreatedAt = timeProvider.GetUtcNow()
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return new UserIdentity(user.Id, user.Name, user.Email);
        }

        if (result.Errors.Any(error => DuplicateErrorCodes.Contains(error.Code)))
        {
            throw new ConflictException("An account with this email already exists");
        }

        throw new ValidationException(result.Errors.Select(error => new ValidationFailure(PropertyFor(error), error.Description)));
    }

    public async Task<UserIdentity> ValidateCredentialsAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null || !await userManager.CheckPasswordAsync(user, password))
        {
            return null;
        }

        return new UserIdentity(user.Id, user.Name, user.Email);
    }

    public Task<UserIdentity> FindByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return userManager.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => new UserIdentity(user.Id, user.Name, user.Email))
            .SingleOrDefaultAsync(cancellationToken);
    }

    private static string PropertyFor(IdentityError error)
    {
        return error.Code.StartsWith("Password", StringComparison.Ordinal) ? "Password" : "Email";
    }
}
