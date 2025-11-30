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

    // 1. Enqueue
    [HttpPost("enqueue")]
    public async Task<IActionResult> Enqueue([FromBody] EnqueueJobRequest request, CancellationToken ct)
    {
        var id = await _jobService.EnqueueAsync(request, ct);
        return Accepted(new { JobId = id });
    }

    // 2. Fetch next job
    [HttpPost("fetch")]
    public async Task<IActionResult> FetchNext([FromBody] FetchJobRequest request, CancellationToken ct)
    {
        var job = await _jobService.FetchNextJobAsync(request.WorkerId, ct);
        if (job == null)
            return NoContent();

        return Ok(job);
    }

    // 3. Mark succeeded
    [HttpPost("{id:guid}/success")]
    public async Task<IActionResult> MarkSuccess(Guid id, CancellationToken ct)
    {
        await _jobService.MarkSucceededAsync(id, ct);
        return Ok();
    }

    // 4. Mark failed
    [HttpPost("{id:guid}/fail")]
    public async Task<IActionResult> MarkFailed(Guid id, [FromBody] FailJobRequest request, CancellationToken ct)
    {
        await _jobService.MarkFailedAsync(id, request.Error, ct);
        return Ok();
    }
}

public class FetchJobRequest
{
    public string WorkerId { get; set; } = default!;
}

public class FailJobRequest
{
    public string Error { get; set; } = default!;
}
