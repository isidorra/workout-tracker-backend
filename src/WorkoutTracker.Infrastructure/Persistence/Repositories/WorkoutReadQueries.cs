using Microsoft.EntityFrameworkCore;

using WorkoutTracker.Application.Features.Dashboard;

namespace WorkoutTracker.Infrastructure.Persistence.Repositories;

public sealed class WorkoutReadQueries(AppDbContext context) : IWorkoutReadQueries
{
    public Task<LastWorkoutSummary> GetLastByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return context.Workouts
            .AsNoTracking()
            .Where(workout => workout.UserId == userId)
            .OrderByDescending(workout => workout.PerformedAt)
            .ThenByDescending(workout => workout.CreatedAt)
            .Select(workout => new LastWorkoutSummary(
                workout.Id,
                workout.Type,
                workout.PerformedAt,
                workout.DurationMinutes))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WorkoutStat>> GetStatsByUserInRangeAsync(
        Guid userId,
        DateTime fromInclusive,
        DateTime toExclusive,
        CancellationToken cancellationToken = default)
    {
        return await context.Workouts
            .AsNoTracking()
            .Where(workout =>
                workout.UserId == userId &&
                workout.PerformedAt >= fromInclusive &&
                workout.PerformedAt < toExclusive)
            .Select(workout => new WorkoutStat(
                workout.PerformedAt,
                workout.DurationMinutes,
                workout.Difficulty,
                workout.Fatigue))
            .ToListAsync(cancellationToken);
    }
}
