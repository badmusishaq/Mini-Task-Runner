using System.Text.Json;
using MiniTaskRunner.Core.Abstractions;

public class ReportGenerationHandler : IJobHandler
{
    public string JobType => "Report";

    public Task HandleAsync(string payload)
    {
        var data = JsonSerializer.Deserialize<ReportPayload>(payload);

        Console.WriteLine($"Generating report: {data!.ReportName}");

        // Simulate heavy work
        Thread.Sleep(2000);

        Console.WriteLine($"Report '{data.ReportName}' generated successfully");

        return Task.CompletedTask;
    }
}

public class ReportPayload
{
    public string ReportName { get; set; } = default!;
}
