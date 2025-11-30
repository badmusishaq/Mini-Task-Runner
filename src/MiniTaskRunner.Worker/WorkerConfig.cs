public class WorkerConfig
{
    public string ApiBaseUrl { get; set; } = "http://localhost:5126";
    public string WorkerId { get; set; } = Guid.NewGuid().ToString();
    public int PollIntervalMs { get; set; } = 2000;
}
