using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniTaskRunner.Core.Domain;
using MiniTaskRunner.Infrastructure.Persistence;
using MiniTaskRunner.Infrastructure.Services;
using Xunit;

namespace MiniTaskRunner.Tests;

public class JobService_FetchTests
{
    private JobDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new JobDbContext(options);
    }

    [Fact]
    public async Task FetchNextJob_Should_Return_Pending_Job()
    {
        // Arrange
        var db = CreateDb();
        var service = new JobService(db);

        var job = new Job
        {
            Id = Guid.NewGuid(),
            Type = "Email",
            Payload = "{}",
            Priority = 0,
            Status = JobStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        db.Jobs.Add(job);
        await db.SaveChangesAsync();

        // Act
        var fetched = await service.FetchNextJobAsync("worker-1");

        // Assert
        Assert.NotNull(fetched);
        Assert.Equal(job.Id, fetched!.Id);
        Assert.Equal(JobStatus.Processing, fetched.Status);
    }
}
