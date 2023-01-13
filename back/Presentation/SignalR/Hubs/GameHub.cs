using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Presentation.Context;
using Presentation.Domain;
using Presentation.Dto;
using Presentation.Entities;
using Presentation.RabbitMq;
using Presentation.SignalR.Clients;

namespace Presentation.SignalR.Hubs;

public class GameHub : Hub<IGameClient>
{
    private readonly GameUpdateProducer _gameUpdateProducer;
    private readonly PostgresDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public GameHub(GameUpdateProducer gameUpdateProducer, PostgresDbContext dbContext, UserManager<User> userManager)
    {
        _gameUpdateProducer = gameUpdateProducer;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    [Authorize]
    public async Task Join(Guid gameId)
    {
        var game = await _dbContext.Games.FirstOrDefaultAsync(game => game.Id == gameId);

        if (game is { Status: GameStatus.New } and ({ PlayerO: null } or { PlayerX: null }))
        {
            var user = await _userManager.GetUserAsync(Context.User!);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            await Clients.Group(gameId.ToString()).UpdateGame(new GameDto(game));
        }
    }

    public async Task Watch(Guid gameId)
    {
        var game = await _dbContext.Games.FirstOrDefaultAsync(game => game.Id == gameId);

        if (game is { })
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            await Clients.Group(gameId.ToString()).UpdateGame(new GameDto(game));
        }
    }

    public async Task PlaceFigure(int first, int second, Guid gameId, Figure figure)
    {
        var game = await _dbContext.Games.FirstOrDefaultAsync(game => game.Id == gameId);

        if (game is null) return;

        if (game[first, second] != Figure.None || GameDomain.WhoseMove(game) != figure)
            return;

        game[first, second] = figure;
        await Clients.Group(gameId.ToString()).UpdateGame(new GameDto(game));

        if (GameDomain.IsGameFinished(game))
        {
            var winner = GameDomain.IsSomeoneWon(game);
            await Clients.Group(gameId.ToString()).GameFinish(winner);
        }

        //_gameUpdateProducer.ProduceGameUpdateCommand(game);
        _dbContext.Games.Update(game);
        await _dbContext.SaveChangesAsync();
    }
}