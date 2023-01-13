using Back.Entities;
using Back.Hubs.Clients;
using Back.Services;
using Microsoft.AspNetCore.SignalR;

namespace Back.Hubs;

public class ChatHub : Hub<IChatClient>
{
    private readonly MessagesService _messagesService;

    public ChatHub(MessagesService messagesService) => 
        _messagesService = messagesService;

    public async Task SendMessage(Message message) => 
        await Clients.All.ReceiveMessage(await _messagesService.AddMessage(message));
}