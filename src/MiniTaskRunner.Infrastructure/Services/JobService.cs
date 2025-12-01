using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniTaskRunner.Core.Abstractions;
using MiniTaskRunner.Core.Domain;
using MiniTaskRunner.Infrastructure.Persistence;
using System.Text.Json;

public class JobService : IJobService
{
    private readonly JobDbContext _db;
    private readonly ILogger<JobService> _logger;

    public JobService(JobDbContext db, ILogger<JobService> logger)
    {
        _db = db;
        _logger = logger;
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

        _logger.LogInformation(
            "Enqueuing job {JobId} of type {JobType} with priority {Priority} scheduled for {ScheduledAt}",
            job.Id, job.Type, job.Priority, job.ScheduledAt);

        _db.Jobs.Add(job);
        await _db.SaveChangesAsync(ct);

        return job.Id;
    }

    public async Task<Job?> FetchNextJobAsync(string workerId, CancellationToken ct = default)
    {
        _logger.LogInformation("Worker {WorkerId} fetching next job", workerId);

        var job = await _db.Jobs
            .Where(j => j.Status == JobStatus.Pending &&
                        (j.ScheduledAt == null || j.ScheduledAt <= DateTime.UtcNow))
            .OrderByDescending(j => j.Priority)
            .ThenBy(j => j.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (job == null)
        {
            _logger.LogInformation("No pending jobs available for worker {WorkerId}", workerId);
            return null;
        }

        job.Status = JobStatus.Processing;

        _logger.LogInformation(
            "Worker {WorkerId} reserved job {JobId} ({JobType})",
            workerId, job.Id, job.Type);

        await _db.SaveChangesAsync(ct);
        return job;
    }

    public async Task MarkSucceededAsync(Guid jobId, CancellationToken ct = default)
    {
        var job = await _db.Jobs.FindAsync(new object[] { jobId }, ct);

        if (job == null)
        {
            _logger.LogWarning("Attempted to mark job {JobId} as succeeded, but it was not found", jobId);
            return;
        }

        job.Status = JobStatus.Succeeded;
        job.CompletedAt = DateTime.UtcNow;

        _logger.LogInformation("Job {JobId} succeeded", jobId);

        await _db.SaveChangesAsync(ct);
    }

    public async Task MarkFailedAsync(Guid jobId, string error, CancellationToken ct = default)
    {
        var job = await _db.Jobs.FindAsync(new object[] { jobId }, ct);

        if (job == null)
        {
            _logger.LogWarning("Attempted to mark job {JobId} as failed, but it was not found", jobId);
            return;
        }

        job.AttemptCount++;
        job.LastError = error;

        _logger.LogWarning(
            "Job {JobId} failed on attempt {Attempt}. Error: {Error}",
            jobId, job.AttemptCount, error);

        // Dead-letter if max attempts reached
        if (job.AttemptCount >= job.MaxAttempts)
        {
            job.Status = JobStatus.DeadLettered;
            job.ScheduledAt = null;

            _logger.LogError(
                "Job {JobId} moved to DeadLetter after {Attempts} attempts",
                jobId, job.AttemptCount);

            await _db.SaveChangesAsync(ct);
            return;
        }

        // Otherwise retry with exponential backoff
        var delaySeconds = Math.Pow(2, job.AttemptCount);
        job.ScheduledAt = DateTime.UtcNow.AddSeconds(delaySeconds);
        job.Status = JobStatus.Pending;

        _logger.LogInformation(
            "Job {JobId} scheduled for retry in {DelaySeconds} seconds (Attempt {Attempt})",
            jobId, delaySeconds, job.AttemptCount);

        await _db.SaveChangesAsync(ct);
    }
}
