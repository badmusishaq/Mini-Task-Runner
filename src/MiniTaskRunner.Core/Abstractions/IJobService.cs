/*using MiniTaskRunner.Core.Domain;

namespace MiniTaskRunner.Core.Abstractions;

public interface IJobService
{
    Task<Guid> EnqueueAsync(EnqueueJobRequest request, CancellationToken ct = default);
    Task<Job?> FetchNextJobAsync(string workerId, CancellationToken ct = default);
    Task MarkSucceededAsync(Guid jobId, CancellationToken ct = default);
    Task MarkFailedAsync(Guid jobId, string error, CancellationToken ct = default);
}*/

using MiniTaskRunner.Core.Domain;

namespace MiniTaskRunner.Core.Abstractions;

public interface IJobService
{
    Task<Guid> EnqueueAsync(EnqueueJobRequest request, CancellationToken ct = default);
    Task<Job?> FetchNextJobAsync(string workerId, CancellationToken ct = default);
    Task MarkSucceededAsync(Guid jobId, CancellationToken ct = default);
    Task MarkFailedAsync(Guid jobId, string error, CancellationToken ct = default);
}

