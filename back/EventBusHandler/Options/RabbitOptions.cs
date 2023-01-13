namespace EventBusHandler.Options;

public class RabbitOptions
{
    public const string RabbitConfiguration = "RabbitConfiguration";

    public required string ConnectionString { get; set; }
    public required string HostName { get; set; }
    public required string GameUpdateQueue { get; set; }
}