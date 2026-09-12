using WorkoutTracker.Application.Features.Dashboard;

namespace WorkoutTracker.Application.Tests.Dashboard;

public sealed class DashboardCalendarTests
{
    [Fact]
    public void StartOfWeek_on_monday_stays_monday()
    {
        var monday = new DateOnly(2026, 3, 2);

        Assert.Equal(monday, DashboardCalendar.StartOfWeek(monday));
    }

    [Fact]
    public void StartOfWeek_on_sunday_maps_to_previous_monday()
    {
        var sunday = new DateOnly(2026, 3, 1);

        Assert.Equal(new DateOnly(2026, 2, 23), DashboardCalendar.StartOfWeek(sunday));
    }

    [Fact]
    public void StartOfWeek_on_new_years_day_can_cross_into_previous_year()
    {
        var januaryFirst = new DateOnly(2026, 1, 1);

        Assert.Equal(new DateOnly(2025, 12, 29), DashboardCalendar.StartOfWeek(januaryFirst));
    }

    [Fact]
    public void ToWeekStats_empty_collection_has_zero_totals_and_null_averages()
    {
        var stats = DashboardCalendar.ToWeekStats([]);

        Assert.Equal(0, stats.WorkoutCount);
        Assert.Equal(0, stats.TotalDurationMinutes);
        Assert.Null(stats.AverageDifficulty);
        Assert.Null(stats.AverageFatigue);
    }

    [Fact]
    public void ToWeekStats_two_rows_sums_duration_and_averages_ratings()
    {
        var workouts = new[]
        {
            new WorkoutStat(new DateTime(2026, 3, 2, 10, 0, 0), 40, 6, 4),
            new WorkoutStat(new DateTime(2026, 3, 3, 10, 0, 0), 20, 8, 8)
        };

        var stats = DashboardCalendar.ToWeekStats(workouts);

        Assert.Equal(2, stats.WorkoutCount);
        Assert.Equal(60, stats.TotalDurationMinutes);
        Assert.Equal(7, stats.AverageDifficulty);
        Assert.Equal(6, stats.AverageFatigue);
    }
}
