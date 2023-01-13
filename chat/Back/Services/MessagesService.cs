using Back.Entities;
using Back.RabbitMQ.Producer;
using Back.Repositories;

namespace Back.Services;

public class MessagesService
{
    private readonly MessagesRepository _messagesRepository;
    private readonly IMessageProducer _messagePublisher;

    public MessagesService(MessagesRepository messagesRepository,
        IMessageProducer messagePublisher)
    {
        _messagesRepository = messagesRepository;
        _messagePublisher = messagePublisher;
    }

    public async Task<IEnumerable<Message>> GetLast(int count = 20) => await _messagesRepository.GetLast(count);

    public async Task<Message> AddMessage(Message message)
    {
        _messagePublisher.SendMessage(message);
        return message;
    }

    public async Task<bool> RemoveAll() =>
        await _messagesRepository.RemoveAll();
}