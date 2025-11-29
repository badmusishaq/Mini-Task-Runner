using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniTaskRunner.Core.Domain;
using MiniTaskRunner.Infrastructure.Persistence;
using Xunit;

namespace MiniTaskRunner.Tests;

public class JobServiceTests
{
    private JobDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new JobDbContext(options);
    }

    [Fact]
    public async Task Enqueue_Should_Create_Job_With_Defaults()
    {
        // Arrange
        var db = CreateInMemoryDb();

        var job = new Job
        {
            Id = Guid.NewGuid(),
            Type = "SendEmail",
            Payload = "{ \"to\": \"test@example.com\" }"
        };

        // Act
        db.Jobs.Add(job);
        await db.SaveChangesAsync();

        var saved = await db.Jobs.FirstOrDefaultAsync(j => j.Id == job.Id);

        // Assert
        Assert.NotNull(saved);
        Assert.Equal("SendEmail", saved.Type);
        Assert.Equal(JobStatus.Pending, saved.Status);
        Assert.Equal(0, saved.AttemptCount);
        Assert.True(saved.CreatedAt <= DateTime.UtcNow);
    }
}
