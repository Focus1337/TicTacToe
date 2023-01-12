using EventBusHandler.RabbitMq;

namespace EventBusHandler.HostedServices;

public class GameUpdateBusHandler : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(1));

    private readonly GameUpdateConsumer _gameUpdateConsumer;

    public GameUpdateBusHandler(GameUpdateConsumer gameUpdateConsumer)
    {
        _gameUpdateConsumer = gameUpdateConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            try
            {
                _gameUpdateConsumer.StartConsumingGameUpdateCommands();
            }
            catch
            {
                _gameUpdateConsumer.Dispose();
            }
        } while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested);
    }

    public override void Dispose()
    {
        _gameUpdateConsumer.Dispose();
        base.Dispose();
    }
}