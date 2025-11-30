using Microsoft.AspNetCore.Mvc;
using MiniTaskRunner.Core.Abstractions;
using MiniTaskRunner.Core.Domain;

namespace MiniTaskRunner.Api.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpPost("enqueue")]
    public async Task<IActionResult> Enqueue([FromBody] EnqueueJobRequest request, CancellationToken ct)
    {
        var id = await _jobService.EnqueueAsync(request, ct);
        return Accepted(new { JobId = id });
    }
}
