using System.Text;
using System.Text.Json;
using EventBusHandler.Data;
using EventBusHandler.Entities;
using EventBusHandler.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBusHandler.RabbitMq;

public class GameUpdateConsumer
{
    private readonly IConnection? _connection;
    private readonly IModel _model;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<GameUpdateConsumer> _logger;
    private readonly RabbitOptions _rabbitOptions;

    public GameUpdateConsumer(IServiceProvider serviceProvider, ILogger<GameUpdateConsumer> logger,
        IOptions<RabbitOptions> rabbitOptions, ConnectionFactory connectionFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _rabbitOptions = rabbitOptions.Value;

        _connection = connectionFactory.CreateConnection();
        _model = _connection.CreateModel();
    }

    public void StartConsumingGameUpdateCommands()
    {
        _model.QueueDeclare(queue: _rabbitOptions.GameUpdateQueue, durable: true, exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(_model);
        consumer.Received += async (_, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var body = JsonSerializer.Deserialize<Game>(content);
            if (body is null) return;

            _logger.LogInformation(@"[{TimeNow}] New game update: {GameId} | Status: {Status} | CreatedAt: {CreatedAt}",
                DateTime.Now, body.Id, body.Status, body.CreatedDateTime);

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var game = await context.Games.FirstOrDefaultAsync(x => x.Id == body.Id);
            
           // game.
            
            context.Games.Update(body);
            await context.SaveChangesAsync();
            
            _model.BasicAck(ea.DeliveryTag, false);
        };
        _model.BasicConsume(_rabbitOptions.GameUpdateQueue, false, consumer);
    }

    public void Dispose()
    {
        _model.Dispose();

        if (_connection?.IsOpen ?? false)
            _connection.Dispose();
    }
}