using MessageReceiverAndDbWriter.RabbitMq;

namespace MessageReceiverAndDbWriter.HostedServices;

public class GameUpdateBusHandler : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(1));

    private readonly MessagesConsumer _messagesConsumer;

    public GameUpdateBusHandler(MessagesConsumer messagesConsumer)
    {
        _messagesConsumer = messagesConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            try
            {
                _messagesConsumer.StartConsumingMessages();
            }
            catch
            {
                _messagesConsumer.Dispose();
            }
        } while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);
    }

    public override void Dispose()
    {
        _messagesConsumer.Dispose();
        base.Dispose();
    }
}