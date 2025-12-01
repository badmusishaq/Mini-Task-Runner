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
        [FromQuery] string? search,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortDir = "desc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _db.Jobs.AsQueryable();

        // Filtering
        if (status.HasValue)
            query = query.Where(j => j.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(j => j.Type == type);

        // Search (in payload or type)
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(j =>
                j.Type.Contains(search) ||
                j.Payload.Contains(search));

        // Sorting
        query = (sortBy.ToLower(), sortDir.ToLower()) switch
        {
            ("priority", "asc") => query.OrderBy(j => j.Priority),
            ("priority", "desc") => query.OrderByDescending(j => j.Priority),

            ("createdat", "asc") => query.OrderBy(j => j.CreatedAt),
            ("createdat", "desc") => query.OrderByDescending(j => j.CreatedAt),

            ("status", "asc") => query.OrderBy(j => j.Status),
            ("status", "desc") => query.OrderByDescending(j => j.Status),

            _ => query.OrderByDescending(j => j.CreatedAt)
        };

        // Pagination
        var total = await query.CountAsync();
        var jobs = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(total / (double)pageSize),
            jobs
        });
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
