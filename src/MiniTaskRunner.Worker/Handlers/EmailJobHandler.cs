using System.Text.Json;
using MiniTaskRunner.Core.Abstractions;

public class EmailJobHandler : IJobHandler
{
    public string JobType => "Email";

    public Task HandleAsync(string payload)
    {
        var data = JsonSerializer.Deserialize<EmailPayload>(payload);

        Console.WriteLine($"Sending email to {data!.To} with subject '{data.Subject}'");

        return Task.CompletedTask;
    }
}

public class EmailPayload
{
    public string To { get; set; } = default!;
    public string Subject { get; set; } = default!;
}
