using NSubstitute;

using WorkoutTracker.Application.Common.Interfaces;
using WorkoutTracker.Application.Features.Dashboard;

namespace WorkoutTracker.Application.Tests.Dashboard;

public sealed class GetDashboardProgressHandlerTests
{
    [Fact]
    public async Task Handle_groups_spillover_weeks_and_requests_the_monday_to_sunday_range()
    {
        var userId = Guid.CreateVersion7();
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.UserId.Returns(userId);

        var sundayOnMonthStart = new WorkoutStat(new DateTime(2026, 3, 1, 9, 0, 0), 30, 5, 5);
        var firstMondayOfMonth = new WorkoutStat(new DateTime(2026, 3, 2, 9, 0, 0), 45, 7, 6);

        var queries = Substitute.For<IWorkoutReadQueries>();
        queries.GetStatsByUserInRangeAsync(
                userId,
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>())
            .Returns([sundayOnMonthStart, firstMondayOfMonth]);

        var response = await GetDashboardProgressHandler.Handle(
            new GetDashboardProgressQuery { Year = 2026, Month = 3 },
            currentUser,
            queries,
            CancellationToken.None);

        await queries.Received(1).GetStatsByUserInRangeAsync(
            userId,
            new DateTime(2026, 2, 23),
            new DateTime(2026, 4, 6),
            Arg.Any<CancellationToken>());

        Assert.Equal(2026, response.Year);
        Assert.Equal(3, response.Month);
        Assert.Equal(6, response.Weeks.Count);

        var firstWeek = response.Weeks[0];
        Assert.Equal(new DateOnly(2026, 2, 23), firstWeek.WeekStart);
        Assert.Equal(new DateOnly(2026, 3, 1), firstWeek.WeekEnd);
        Assert.Equal(1, firstWeek.WorkoutCount);
        Assert.Equal(30, firstWeek.TotalDurationMinutes);
        Assert.Equal(5, firstWeek.AverageDifficulty);

        var secondWeek = response.Weeks[1];
        Assert.Equal(new DateOnly(2026, 3, 2), secondWeek.WeekStart);
        Assert.Equal(1, secondWeek.WorkoutCount);
        Assert.Equal(45, secondWeek.TotalDurationMinutes);

        Assert.All(
            response.Weeks.Skip(2),
            week =>
            {
                Assert.Equal(0, week.WorkoutCount);
                Assert.Null(week.AverageDifficulty);
                Assert.Null(week.AverageFatigue);
            });
    }
}
