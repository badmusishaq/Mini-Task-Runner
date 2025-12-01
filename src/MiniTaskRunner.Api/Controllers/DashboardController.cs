using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniTaskRunner.Infrastructure.Persistence;
using MiniTaskRunner.Core.Domain;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly JobDbContext _db;

    public DashboardController(JobDbContext db)
    {
        _db = db;
    }

    [HttpGet("jobs")]
    public async Task<IActionResult> GetJobs(
        [FromQuery] JobStatus? status,
        [FromQuery] string? type,
        [FromQuery] int limit = 100)
    {
        var query = _db.Jobs.AsQueryable();

        if (status.HasValue)
            query = query.Where(j => j.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(j => j.Type == type);

        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .Take(limit)
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpGet("jobs/{id:guid}")]
    public async Task<IActionResult> GetJob(Guid id)
    {
        var job = await _db.Jobs.FindAsync(id);

        if (job == null)
            return NotFound();

        return Ok(job);
    }

    [HttpPost("jobs/{id:guid}/retry")]
    public async Task<IActionResult> RetryJob(Guid id)
    {
        var job = await _db.Jobs.FindAsync(id);

        if (job == null)
            return NotFound();

        if (job.Status != JobStatus.DeadLettered)
            return BadRequest("Only dead-lettered jobs can be retried.");

        job.Status = JobStatus.Pending;
        job.AttemptCount = 0;
        job.ScheduledAt = null;
        job.LastError = null;

        await _db.SaveChangesAsync();

        return Ok(job);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = new
        {
            Total = await _db.Jobs.CountAsync(),
            Pending = await _db.Jobs.CountAsync(j => j.Status == JobStatus.Pending),
            Processing = await _db.Jobs.CountAsync(j => j.Status == JobStatus.Processing),
            Succeeded = await _db.Jobs.CountAsync(j => j.Status == JobStatus.Succeeded),
            Failed = await _db.Jobs.CountAsync(j => j.Status == JobStatus.Failed),
            DeadLettered = await _db.Jobs.CountAsync(j => j.Status == JobStatus.DeadLettered)
        };

        return Ok(stats);
    }

    [HttpDelete("dead")]
    public async Task<IActionResult> ClearDeadLettered()
    {
        var dead = _db.Jobs.Where(j => j.Status == JobStatus.DeadLettered);

        _db.Jobs.RemoveRange(dead);
        await _db.SaveChangesAsync();

        return NoContent();
    }

}
