using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniTaskRunner.Core.Domain;
using MiniTaskRunner.Infrastructure.Persistence;
using MiniTaskRunner.Infrastructure.Services;
using Xunit;

namespace MiniTaskRunner.Tests;

public class JobService_OtherTests
{
    private JobDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new JobDbContext(options);
    }

    [Fact]
    public async Task EnqueueAsync_Should_Save_Job()
    {
        // Arrange
        var db = CreateDb();
        var service = new JobService(db);

        var request = new EnqueueJobRequest
        {
            Type = "Email",
            Payload = new { To = "test@example.com" },
            Priority = 2
        };

        // Act
        var jobId = await service.EnqueueAsync(request);

        // Assert
        var job = await db.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);

        Assert.NotNull(job);
        Assert.Equal("Email", job!.Type);
        Assert.Equal(JobStatus.Pending, job.Status);
        Assert.Equal(2, job.Priority);
        Assert.Equal(0, job.AttemptCount);
        Assert.NotNull(job.CreatedAt);
        Assert.NotEmpty(job.Payload);
    }

    [Fact]
    public async Task MarkSucceeded_Should_Update_Status()
    {
        // Arrange
        var db = CreateDb();
        var service = new JobService(db);

        var job = new Job
        {
            Id = Guid.NewGuid(),
            Type = "Email",
            Payload = "{}",
            Status = JobStatus.Processing,
            CreatedAt = DateTime.UtcNow
        };

        db.Jobs.Add(job);
        await db.SaveChangesAsync();

        // Act
        await service.MarkSucceededAsync(job.Id);

        // Assert
        var updated = await db.Jobs.FindAsync(job.Id);

        Assert.NotNull(updated);
        Assert.Equal(JobStatus.Succeeded, updated!.Status);
        Assert.NotNull(updated.CompletedAt);
    }

    [Fact]
    public async Task MarkFailed_Should_Apply_Backoff()
    {
        // Arrange
        var db = CreateDb();
        var service = new JobService(db);

        var job = new Job
        {
            Id = Guid.NewGuid(),
            Type = "Email",
            Payload = "{}",
            Status = JobStatus.Processing,
            AttemptCount = 0,
            MaxAttempts = 5,
            CreatedAt = DateTime.UtcNow
        };

        db.Jobs.Add(job);
        await db.SaveChangesAsync();

        // Act
        await service.MarkFailedAsync(job.Id, "Something went wrong");

        // Assert
        var updated = await db.Jobs.FindAsync(job.Id);

        Assert.NotNull(updated);
        Assert.Equal(1, updated!.AttemptCount);
        Assert.Equal(JobStatus.Pending, updated.Status);
        Assert.NotNull(updated.ScheduledAt);
        Assert.True(updated.ScheduledAt > DateTime.UtcNow);
        Assert.Equal("Something went wrong", updated.LastError);
    }
    
    [Fact]
    public async Task MarkFailed_Should_DeadLetter_After_MaxAttempts()
    {
        // Arrange
        var db = CreateDb();
        var service = new JobService(db);

        var job = new Job
        {
            Id = Guid.NewGuid(),
            Type = "Email",
            Payload = "{}",
            Status = JobStatus.Processing,
            AttemptCount = 4,
            MaxAttempts = 5,
            CreatedAt = DateTime.UtcNow
        };

        db.Jobs.Add(job);
        await db.SaveChangesAsync();

        // Act
        await service.MarkFailedAsync(job.Id, "Final failure");

        // Assert
        var updated = await db.Jobs.FindAsync(job.Id);

        Assert.NotNull(updated);
        Assert.Equal(JobStatus.DeadLettered, updated!.Status);
        Assert.Equal(5, updated.AttemptCount);
        Assert.Equal("Final failure", updated.LastError);
        Assert.Null(updated.ScheduledAt); // No more retries
    }
}
