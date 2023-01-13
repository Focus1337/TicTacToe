using Presentation.Dto;
using Presentation.Entities;

namespace Presentation.SignalR.Clients;

public interface IGameClient
{
    Task UpdateGame(GameDto game);
    Task GameFinish(Figure winner);
}