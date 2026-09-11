using FluentValidation;

using WorkoutTracker.Application.Common.Exceptions;
using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Domain.Common;
using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Application.Features.Workouts;

public sealed record UpdateWorkoutCommand(
    Guid Id,
    WorkoutType Type,
    DateTime PerformedAt,
    int DurationMinutes,
    int Calories,
    int Difficulty,
    int Fatigue,
    string Notes) : IWorkoutInput;

public sealed class UpdateWorkoutCommandValidator : AbstractValidator<UpdateWorkoutCommand>
{
    public UpdateWorkoutCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        this.AddWorkoutInputRules();
    }
}

public static class UpdateWorkoutHandler
{
    public static async Task<WorkoutResponse> Handle(
        UpdateWorkoutCommand command,
        ICurrentUser currentUser,
        IWorkoutRepository workoutRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var workout = await workoutRepository.GetByIdAsync(command.Id, currentUser.UserId, cancellationToken);

        if (workout is null)
        {
            throw new NotFoundException("Workout not found");
        }

        workout.Update(
            command.Type,
            command.PerformedAt,
            command.DurationMinutes,
            command.Calories,
            command.Difficulty,
            command.Fatigue,
            command.Notes,
            timeProvider.GetUtcNow());

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return workout.ToResponse();
    }
}
