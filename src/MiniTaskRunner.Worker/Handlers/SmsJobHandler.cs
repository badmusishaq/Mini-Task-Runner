using System.Text.Json;
using MiniTaskRunner.Core.Abstractions;

public class SmsJobHandler : IJobHandler
{
    public string JobType => "SMS";

    public Task HandleAsync(string payload)
    {
        var data = JsonSerializer.Deserialize<SmsPayload>(payload);

        Console.WriteLine($"Sending SMS to {data!.PhoneNumber}: {data.Message}");

        // Call Twilio or another SMS provider here.

        return Task.CompletedTask;
    }
}

public class SmsPayload
{
    public string PhoneNumber { get; set; } = default!;
    public string Message { get; set; } = default!;
}
