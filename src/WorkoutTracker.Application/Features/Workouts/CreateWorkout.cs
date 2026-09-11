using FluentValidation;

using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Domain.Common;
using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Application.Features.Workouts;

public sealed record CreateWorkoutCommand(
    WorkoutType Type,
    DateTime PerformedAt,
    int DurationMinutes,
    int Calories,
    int Difficulty,
    int Fatigue,
    string Notes) : IWorkoutInput;

public sealed class CreateWorkoutCommandValidator : AbstractValidator<CreateWorkoutCommand>
{
    public CreateWorkoutCommandValidator()
    {
        this.AddWorkoutInputRules();
    }
}

public static class CreateWorkoutHandler
{
    public static async Task<WorkoutResponse> Handle(
        CreateWorkoutCommand command,
        ICurrentUser currentUser,
        IWorkoutRepository workoutRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var workout = Workout.Create(
            currentUser.UserId,
            command.Type,
            command.PerformedAt,
            command.DurationMinutes,
            command.Calories,
            command.Difficulty,
            command.Fatigue,
            command.Notes,
            timeProvider.GetUtcNow());

        workoutRepository.Add(workout);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return workout.ToResponse();
    }
}
