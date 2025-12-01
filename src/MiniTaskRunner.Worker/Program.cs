using System.Net.Http.Json;
using MiniTaskRunner.Core.Abstractions;
using MiniTaskRunner.Worker;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/worker.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

Log.Information("Worker starting up");

var config = new WorkerConfig();
var http = new HttpClient { BaseAddress = new Uri(config.ApiBaseUrl) };

// Register handlers
var handlers = new List<IJobHandler>
{
    new EmailJobHandler(),
    new SmsJobHandler(),
    new WebhookJobHandler(),
    new ReportGenerationHandler()
};

//Console.WriteLine($"Worker started. ID = {config.WorkerId}");
Log.Information("Worker started: {ID}", config.WorkerId);

while (true)
{
    try
    {
        // Fetch next job
        var fetchResponse = await http.PostAsJsonAsync("/api/jobs/fetch", new
        {
            WorkerId = config.WorkerId
        });

        if (fetchResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            await Task.Delay(config.PollIntervalMs);
            continue;
        }

        var job = await fetchResponse.Content.ReadFromJsonAsync<JobDto>();

        if (job == null)
        {
            await Task.Delay(config.PollIntervalMs);
            continue;
        }

        //Console.WriteLine($"Fetched job {job.Id} ({job.Type})");
        Log.Information("Fetched job {JobId} ({JobType})", job.Id, job.Type);

        // Find handler
        var handler = handlers.FirstOrDefault(h => h.JobType == job.Type);

        if (handler == null)
        {
            //Console.WriteLine($"No handler found for job type {job.Type}");
            Log.Warning("No handler found for job type {JobType}", job.Type);
            await http.PostAsJsonAsync($"/api/jobs/{job.Id}/fail", new { Error = "No handler found" });
            continue;
        }

        try
        {
            await handler.HandleAsync(job.Payload);
            await http.PostAsync($"/api/jobs/{job.Id}/success", null);
            //Console.WriteLine($"Job {job.Id} succeeded");
            Log.Information("Job {JobId} succeeded", job.Id);
        }
        catch (Exception ex)
        {
            await http.PostAsJsonAsync($"/api/jobs/{job.Id}/fail", new { Error = ex.Message });
            //Console.WriteLine($"Job {job.Id} failed: {ex.Message}");
            Log.Error("Job {JobId} failed: {Error}", job.Id, ex.Message);
        }
    }
    catch (Exception ex)
    {
        //Console.WriteLine($"Worker error: {ex.Message}");
        Log.Error("Worker error: {Message}", ex.Message);
    }

    await Task.Delay(config.PollIntervalMs);
}

public class JobDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
}

