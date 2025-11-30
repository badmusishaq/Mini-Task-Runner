namespace MiniTaskRunner.Core.Domain;

public class EnqueueJobRequest
{
    public string Type { get; set; } = default!;
    public object Payload { get; set; } = default!;
    public int Priority { get; set; } = 0;
    public DateTime? ScheduledAt { get; set; }
}
