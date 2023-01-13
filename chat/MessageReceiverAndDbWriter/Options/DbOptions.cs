namespace MessageReceiverAndDbWriter.Options;

public class DbOptions
{
    public const string DbConfiguration = "DbConfiguration";

    public string ConnectionString { get; set; } = null!;
}