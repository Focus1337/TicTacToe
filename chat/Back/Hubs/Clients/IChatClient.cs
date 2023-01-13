using Back.Entities;

namespace Back.Hubs.Clients;

public interface IChatClient
{
    Task ReceiveMessage(Message message);
}