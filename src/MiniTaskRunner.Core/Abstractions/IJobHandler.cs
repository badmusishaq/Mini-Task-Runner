namespace MiniTaskRunner.Core.Abstractions;

public interface IJobHandler
{
    string JobType { get; }
    Task HandleAsync(string payload);
}
