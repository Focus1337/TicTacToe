using Presentation.Entities;

namespace Presentation.SignalR.Clients;

public interface IGameClient
{
    Task UpdateGame(Game game);
    Task GameFinish(Figure winner);
}