using Microsoft.AspNetCore.Mvc;
using Scheduler.Application.Abstractions;
using Scheduler.Application.DTOs;

namespace Gentongku.Api.Controllers;

[ApiController]
[Route("api/scheduler")]
public class SchedulerController : ControllerBase
{
    private readonly ISchedulerService _schedulerService;

    public SchedulerController(ISchedulerService schedulerService) => _schedulerService = schedulerService;

    [HttpPost("generate-dummy-data")]
    public async Task<IActionResult> GenerateDummyData(GenerateDummyDataRequest request, CancellationToken ct)
    {
        var hangfireJobId = await _schedulerService.EnqueueGenerateDummyDataAsync(request, ct);
        return Accepted(new { hangfireJobId });
    }

    [HttpGet("jobs")]
    public async Task<IActionResult> ListJobs(CancellationToken ct) => Ok(await _schedulerService.ListRecentJobsAsync(20, ct));
}
