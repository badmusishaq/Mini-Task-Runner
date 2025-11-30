using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MiniTaskRunner.Core.Abstractions;
using MiniTaskRunner.Core.Domain;
using MiniTaskRunner.Infrastructure.Persistence;

namespace MiniTaskRunner.Infrastructure.Services;

public class JobService : IJobService
{
    private readonly JobDbContext _db;

    public JobService(JobDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> EnqueueAsync(EnqueueJobRequest request, CancellationToken ct = default)
    {
        var job = new Job
        {
            Id = Guid.NewGuid(),
            Type = request.Type,
            Payload = JsonSerializer.Serialize(request.Payload),
            Priority = request.Priority,
            ScheduledAt = request.ScheduledAt,
            Status = JobStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _db.Jobs.Add(job);
        await _db.SaveChangesAsync(ct);

        return job.Id;
    }

    /*public async Task<Job?> FetchNextJobAsync(string workerId, CancellationToken ct = default)
    {
        // Atomic fetch using a transaction
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var job = await _db.Jobs
            .Where(j => j.Status == JobStatus.Pending &&
                        (j.ScheduledAt == null || j.ScheduledAt <= DateTime.UtcNow))
            .OrderBy(j => j.Priority)
            .ThenBy(j => j.ScheduledAt)
            .ThenBy(j => j.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (job == null)
            return null;

        job.Status = JobStatus.Processing;
        job.LastAttemptAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return job;
    }*/

    public async Task<Job?> FetchNextJobAsync(string workerId, CancellationToken ct = default)
{
    var job = await _db.Jobs
        .Where(j => j.Status == JobStatus.Pending &&
                    (j.ScheduledAt == null || j.ScheduledAt <= DateTime.UtcNow))
        .OrderBy(j => j.Priority)
        .ThenBy(j => j.ScheduledAt)
        .ThenBy(j => j.CreatedAt)
        .FirstOrDefaultAsync(ct);

    if (job == null)
        return null;

    job.Status = JobStatus.Processing;
    job.LastAttemptAt = DateTime.UtcNow;

    await _db.SaveChangesAsync(ct);

    return job;
}

    public async Task MarkSucceededAsync(Guid jobId, CancellationToken ct = default)
    {
        var job = await _db.Jobs.FindAsync(new object[] { jobId }, ct);
        if (job == null) return;

        job.Status = JobStatus.Succeeded;
        job.CompletedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
    }

    public async Task MarkFailedAsync(Guid jobId, string error, CancellationToken ct = default)
    {
        var job = await _db.Jobs.FindAsync(new object[] { jobId }, ct);
        if (job == null) return;

        job.AttemptCount++;
        job.LastError = error;

        if (job.AttemptCount >= job.MaxAttempts)
        {
            job.Status = JobStatus.DeadLettered;
        }
        else
        {
            job.Status = JobStatus.Pending;
            job.ScheduledAt = ComputeBackoff(job.AttemptCount);
        }

        await _db.SaveChangesAsync(ct);
    }

    private static DateTime ComputeBackoff(int attempt)
    {
        var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
        var jitter = TimeSpan.FromMilliseconds(Random.Shared.Next(0, 500));
        return DateTime.UtcNow + delay + jitter;
    }
}
