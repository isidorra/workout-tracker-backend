using WorkoutTracker.Application.Features.Workouts;
using WorkoutTracker.Domain.Workouts;

namespace WorkoutTracker.Application.Tests.Workouts;

public sealed class CreateWorkoutCommandValidatorTests
{
    private readonly CreateWorkoutCommandValidator _validator = new();

    [Fact]
    public void Valid_command_passes()
    {
        var result = _validator.Validate(Valid());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(DateTimeKind.Utc)]
    [InlineData(DateTimeKind.Local)]
    public void PerformedAt_with_time_zone_kind_fails(DateTimeKind kind)
    {
        var command = Valid() with { PerformedAt = new DateTime(2026, 3, 2, 10, 0, 0, kind) };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateWorkoutCommand.PerformedAt));
    }

    [Fact]
    public void PerformedAt_unspecified_passes()
    {
        var command = Valid() with { PerformedAt = new DateTime(2026, 3, 2, 10, 0, 0, DateTimeKind.Unspecified) };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(99)]
    public void Type_outside_enum_fails(int type)
    {
        var command = Valid() with { Type = (WorkoutType)type };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateWorkoutCommand.Type));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Difficulty_outside_range_fails(int difficulty)
    {
        var command = Valid() with { Difficulty = difficulty };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateWorkoutCommand.Difficulty));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Fatigue_outside_range_fails(int fatigue)
    {
        var command = Valid() with { Fatigue = fatigue };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateWorkoutCommand.Fatigue));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1441)]
    public void Duration_outside_range_fails(int durationMinutes)
    {
        var command = Valid() with { DurationMinutes = durationMinutes };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateWorkoutCommand.DurationMinutes));
    }

    [Fact]
    public void Notes_longer_than_max_fail()
    {
        var command = Valid() with { Notes = new string('x', WorkoutPolicy.MaxNotesLength + 1) };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateWorkoutCommand.Notes));
    }

    private static CreateWorkoutCommand Valid()
    {
        return new CreateWorkoutCommand(
            WorkoutType.Strength,
            new DateTime(2026, 3, 2, 10, 0, 0, DateTimeKind.Unspecified),
            45,
            300,
            6,
            4,
            "felt good");
    }
}
