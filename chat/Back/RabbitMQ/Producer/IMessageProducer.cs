using Back.Entities;

namespace Back.RabbitMQ.Producer;

public interface IMessageProducer
{
    void SendMessage(Message message);
}