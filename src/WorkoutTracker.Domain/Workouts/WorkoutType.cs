namespace WorkoutTracker.Domain.Workouts;

// Values are persisted and sent to clients as numbers, so they must never be renumbered.
// Starting at 1 keeps the default (0) invalid, which catches a missing value.
public enum WorkoutType
{
    Cardio = 1,
    Strength = 2,
    Flexibility = 3
}
