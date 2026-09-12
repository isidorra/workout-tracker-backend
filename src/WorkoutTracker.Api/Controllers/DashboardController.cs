using Microsoft.AspNetCore.Mvc;

using Wolverine;

using WorkoutTracker.Application.Features.Dashboard;

namespace WorkoutTracker.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController(IMessageBus bus) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> Get(
        [FromQuery] GetDashboardQuery query,
        CancellationToken cancellationToken)
    {
        var dashboard = await bus.InvokeAsync<DashboardResponse>(query, cancellationToken);

        return Ok(dashboard);
    }

    [HttpGet("progress")]
    public async Task<ActionResult<DashboardProgressResponse>> GetProgress(
        [FromQuery] GetDashboardProgressQuery query,
        CancellationToken cancellationToken)
    {
        var progress = await bus.InvokeAsync<DashboardProgressResponse>(query, cancellationToken);

        return Ok(progress);
    }
}
