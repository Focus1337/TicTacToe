using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using Presentation.Data;
using Presentation.Domain;
using Presentation.Entities;
using Presentation.RabbitMq;
using Presentation.SignalR.Clients;

namespace Presentation.SignalR.Hubs;

public class GameHub : Hub<IGameClient>
{
    private readonly MongoDbContext _dbContext;
    private readonly GameUpdateProducer _gameUpdateProducer;

    public GameHub(MongoDbContext dbContext, GameUpdateProducer gameUpdateProducer)
    {
        _dbContext = dbContext;
        _gameUpdateProducer = gameUpdateProducer;
    }

    public async Task Enter(string gameId)
    {
        var cursor = await _dbContext.GetGameCollection().FindAsync(game => game.Id == gameId);
        var game = await cursor.FirstOrDefaultAsync();

        if (game is { })
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Group(gameId).UpdateGame(game);
        }
    }

    public async Task PlaceFigure(int first, int second, string gameId, Figure figure)
    {
        var cursor = await _dbContext.GetGameCollection().FindAsync(game => game.Id == gameId);
        var game = await cursor.FirstOrDefaultAsync();

        if (game is null) return;

        if (game.Cells[first][second] != Figure.None || GameDomain.WhoseMove(game) != figure)
            return;

        game.Cells[first][second] = figure;
        await Clients.Group(gameId).UpdateGame(game);

        if (GameDomain.IsGameFinished(game))
        {
            var winner = GameDomain.IsSomeoneWon(game);
            await Clients.Group(gameId).GameFinish(winner);
        }

        _gameUpdateProducer.ProduceGameUpdateCommand(game);
    }
}