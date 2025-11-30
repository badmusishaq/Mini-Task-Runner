namespace MiniTaskRunner.Core.Domain;

public class Class1
{

}


public enum JobStatus
{
    Pending,
    Processing,
    Succeeded,
    Failed,
    DeadLettered
}

public class Job
{
    public Guid Id { get; set; }
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public int Priority { get; set; } = 0;
    public int AttemptCount { get; set; } = 0;
    public int MaxAttempts { get; set; } = 5;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ScheduledAt { get; set; }
    public DateTime? LastAttemptAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? LastError { get; set; }
}

