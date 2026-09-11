namespace WorkoutTracker.Application.Features.Workouts;

public static class WorkoutPolicy
{
    public const int MinRating = 1;
    public const int MaxRating = 10;

    public const int MinDurationMinutes = 1;
    public const int MaxDurationMinutes = 1440;

    public const int MinCalories = 0;
    public const int MaxCalories = 10000;

    public const int MaxNotesLength = 1000;

    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 50;
}
