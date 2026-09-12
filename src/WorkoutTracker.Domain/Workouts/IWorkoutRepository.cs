namespace WorkoutTracker.Domain.Workouts;

public interface IWorkoutRepository
{
    Task<Workout> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Workout> Items, int TotalCount)> GetByUserAsync(
        Guid userId,
        WorkoutType? type,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Workout> GetLastByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Workout>> GetByUserInRangeAsync(
        Guid userId,
        DateTime fromInclusive,
        DateTime toExclusive,
        CancellationToken cancellationToken = default);

    void Add(Workout workout);
    void Remove(Workout workout);
}
