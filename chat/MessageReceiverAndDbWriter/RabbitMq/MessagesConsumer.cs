using System.Text;
using System.Text.Json;
using MessageReceiverAndDbWriter.Data;
using MessageReceiverAndDbWriter.Entities;
using MessageReceiverAndDbWriter.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageReceiverAndDbWriter.RabbitMq;

public class MessagesConsumer
{
    private readonly IConnection? _connection;
    private readonly IModel _model;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessagesConsumer> _logger;
    private readonly RabbitOptions _rabbitOptions;

    public MessagesConsumer(IServiceProvider serviceProvider, ILogger<MessagesConsumer> logger,
        IOptions<RabbitOptions> rabbitOptions, ConnectionFactory connectionFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _rabbitOptions = rabbitOptions.Value;

        _connection = connectionFactory.CreateConnection();
        _model = _connection.CreateModel();
    }

    public void StartConsumingMessages()
    {
        _model.QueueDeclare(queue: _rabbitOptions.MessagesQueue, durable: true, exclusive: false,
            autoDelete: false,
            arguments: null);

        _logger.LogInformation("Connection opened");

        var consumer = new AsyncEventingBasicConsumer(_model);
        consumer.Received += async (_, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            var body = JsonSerializer.Deserialize<Message>(content);
            if (body is null)
            {
                _logger.LogError("Broken message: \'{Content}\'", content);
                return;
            }

            _logger.LogInformation("New message: {BodyUserName} says \'{BodyText}\'", body.UserName, body.Text);

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await context.Messages.AddAsync(body);
            await context.SaveChangesAsync();

            _model.BasicAck(ea.DeliveryTag, false);
            await Task.Yield();
        };
        _model.BasicConsume(_rabbitOptions.MessagesQueue, false, consumer);
    }

    public void Dispose()
    {
        _model.Dispose();

        if (_connection?.IsOpen ?? false)
            _connection.Dispose();
    }
}