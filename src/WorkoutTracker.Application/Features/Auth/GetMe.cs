using WorkoutTracker.Application.Common.Exceptions;
using WorkoutTracker.Application.Common.Interfaces;

namespace WorkoutTracker.Application.Features.Auth;

public sealed record GetMeQuery();

public sealed record MeResponse(Guid Id, string Name, string Email);

public static class GetMeHandler
{
    public static async Task<MeResponse> Handle(
        GetMeQuery query,
        ICurrentUser currentUser,
        IIdentityService identityService,
        CancellationToken cancellationToken)
    {
        var user = await identityService.FindByIdAsync(currentUser.UserId, cancellationToken);

        if (user is null)
        {
            throw new AuthenticationFailedException("The account no longer exists");
        }

        return new MeResponse(user.Id, user.Name, user.Email);
    }
}
