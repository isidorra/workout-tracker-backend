namespace WorkoutTracker.Application.Features.Dashboard;

public interface IWorkoutReadQueries
{
    Task<LastWorkoutSummary> GetLastByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkoutStat>> GetStatsByUserInRangeAsync(
        Guid userId,
        DateTime fromInclusive,
        DateTime toExclusive,
        CancellationToken cancellationToken = default);
}
