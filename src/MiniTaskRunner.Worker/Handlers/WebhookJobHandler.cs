using System.Net.Http.Json;
using System.Text.Json;
using MiniTaskRunner.Core.Abstractions;

public class WebhookJobHandler : IJobHandler
{
    private readonly HttpClient _http = new();

    public string JobType => "Webhook";

    public async Task HandleAsync(string payload)
    {
        var data = JsonSerializer.Deserialize<WebhookPayload>(payload);

        Console.WriteLine($"Calling webhook: {data!.Url}");

        var response = await _http.PostAsJsonAsync(data.Url, data.Body);

        response.EnsureSuccessStatusCode();

        Console.WriteLine($"Webhook call succeeded: {data.Url}");
    }
}

public class WebhookPayload
{
    public string Url { get; set; } = default!;
    public object Body { get; set; } = default!;
}
