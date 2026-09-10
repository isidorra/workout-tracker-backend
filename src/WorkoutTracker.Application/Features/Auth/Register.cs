using FluentValidation;

using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Domain.Auth;
using WorkoutTracker.Domain.Common;

namespace WorkoutTracker.Application.Features.Auth;

public sealed record RegisterCommand(string Name, string Email, string Password);

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(PasswordPolicy.MinimumLength)
            .MaximumLength(PasswordPolicy.MaximumLength);
    }
}

public static class RegisterHandler
{
    public static async Task<AuthResult> Handle(
        RegisterCommand command,
        IIdentityService identityService,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var user = await identityService.RegisterAsync(command.Name.Trim(), command.Email, command.Password);

        return await AuthTokens.IssueAsync(
            user,
            tokenService,
            refreshTokenRepository,
            unitOfWork,
            timeProvider,
            cancellationToken);
    }
}
