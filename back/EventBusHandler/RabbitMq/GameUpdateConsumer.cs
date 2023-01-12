using System.Text;
using System.Text.Json;
using EventBusHandler.Data;
using EventBusHandler.Entities;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBusHandler.RabbitMq;

public class GameUpdateConsumer
{
    private readonly IConnection? _connection;
    private readonly IModel _model;
    private readonly MongoDbContext _dbContext;

    public GameUpdateConsumer(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
        var factory = new ConnectionFactory { HostName = "rabbit" };
        _connection = factory.CreateConnection();
        _model = _connection.CreateModel();
    }

    public void StartConsumingGameUpdateCommands()
    {
        _model.QueueDeclare(queue: "gameUpdate", durable: true, exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(_model);
        consumer.Received += (_, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var game = JsonSerializer.Deserialize<Game>(content);
            if (game is null) return;

            _dbContext.GetGameCollection().FindOneAndUpdate(
                new ExpressionFilterDefinition<Game>(g => g.Id == game.Id),
                new ObjectUpdateDefinition<Game>(new { game.Cells }));

            _model.BasicAck(ea.DeliveryTag, false);
        };
        _model.BasicConsume("gameUpdate", false, consumer);
    }

    public void Dispose()
    {
        _model.Dispose();

        if (_connection?.IsOpen ?? false)
            _connection.Dispose();
    }
}