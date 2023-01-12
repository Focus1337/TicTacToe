using System.Text;
using Newtonsoft.Json;
using Presentation.Entities;
using RabbitMQ.Client;

namespace Presentation.RabbitMq;

public class GameUpdateProducer : IDisposable
{
    private readonly IModel _model;

    public GameUpdateProducer()
    {
        var factory = new ConnectionFactory { HostName = "rabbit" };
        var connection = factory.CreateConnection();
        _model = connection.CreateModel();
    }

    public void ProduceGameUpdateCommand(Game game)
    {
        _model.QueueDeclare(queue: "gameUpdate",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        var json = JsonConvert.SerializeObject(game);
        var body = Encoding.UTF8.GetBytes(json);
        _model.BasicPublish(exchange: "", routingKey: "gameUpdate", body: body);
    }

    public void Dispose() => _model.Dispose();
}