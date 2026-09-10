using FluentValidation;

using WorkoutTracker.Application.Common.Exceptions;
using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Domain.Auth;
using WorkoutTracker.Domain.Common;

namespace WorkoutTracker.Application.Features.Auth;

public sealed record LoginCommand(string Email, string Password);

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}

public static class LoginHandler
{
    public static async Task<AuthResult> Handle(
        LoginCommand command,
        IIdentityService identityService,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var user = await identityService.ValidateCredentialsAsync(command.Email, command.Password);

        if (user is null)
        {
            throw new AuthenticationFailedException("Invalid email or password");
        }

        return await AuthTokens.IssueAsync(
            user,
            tokenService,
            refreshTokenRepository,
            unitOfWork,
            timeProvider,
            cancellationToken);
    }
}
