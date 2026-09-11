using FluentValidation;

using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Application.Features.Workouts;

public interface IWorkoutInput
{
    WorkoutType Type { get; }
    DateTime PerformedAt { get; }
    int DurationMinutes { get; }
    int Calories { get; }
    int Difficulty { get; }
    int Fatigue { get; }
    string Notes { get; }
}

internal static class WorkoutInputRules
{
    public static void AddWorkoutInputRules<T>(this AbstractValidator<T> validator)
        where T : IWorkoutInput
    {
        validator.RuleFor(x => x.Type)
            .IsInEnum();

        // A value with "Z" or an offset deserializes as Utc or Local. Rejecting it keeps the stored
        // time exactly what the user entered, and Npgsql would refuse a Utc value for this column anyway.
        validator.RuleFor(x => x.PerformedAt)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(performedAt => performedAt.Kind == DateTimeKind.Unspecified)
            .WithMessage("'{PropertyName}' must be a local date and time without a time zone offset.");

        validator.RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(WorkoutPolicy.MinDurationMinutes, WorkoutPolicy.MaxDurationMinutes);

        validator.RuleFor(x => x.Calories)
            .InclusiveBetween(WorkoutPolicy.MinCalories, WorkoutPolicy.MaxCalories);

        validator.RuleFor(x => x.Difficulty)
            .InclusiveBetween(WorkoutPolicy.MinRating, WorkoutPolicy.MaxRating);

        validator.RuleFor(x => x.Fatigue)
            .InclusiveBetween(WorkoutPolicy.MinRating, WorkoutPolicy.MaxRating);

        validator.RuleFor(x => x.Notes)
            .MaximumLength(WorkoutPolicy.MaxNotesLength);
    }
}
