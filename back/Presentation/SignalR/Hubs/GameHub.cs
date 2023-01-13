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

    public async Task Join(Guid gameId, Guid userId, Figure player)
    {
        var game = await _dbContext.Games.FirstOrDefaultAsync(game => game.Id == gameId);

        if (game is { Status: GameStatus.New } and ({ PlayerO: null } or { PlayerX: null }))
        {
            if (game.PlayerX != userId && game.PlayerO != userId)
            {
                switch (player)
                {
                    case Figure.X:
                        game.PlayerX ??= userId;
                        break;
                    case Figure.O:
                        game.PlayerO ??= userId;
                        break;
                    case Figure.None:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(player), player, null);
                }
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            await Clients.Group(gameId.ToString()).UpdateGame(new GameDto(game));

            _dbContext.Games.Update(game);
            await _dbContext.SaveChangesAsync();
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

    public async Task PlaceFigure(int first, int second, Guid gameId, Guid userId)
    {
        var game = await _dbContext.Games.FirstOrDefaultAsync(game => game.Id == gameId);

        if (game is null or { Status: GameStatus.Finished } || game[first, second] != Figure.None) return;
        
        var whoseMove = GameDomain.WhoseMove(game);
        switch (whoseMove)
        {
            case Figure.X:
                if (game.PlayerX != userId)
                    return;
                break;
            case Figure.O:
                if (game.PlayerO != userId)
                    return;
                break;
            case Figure.None:
            default:
                return;
        }

        game[first, second] = whoseMove;

        if (GameDomain.IsGameFinished(game))
        {
            game.Status = GameStatus.Finished;
            var winner = GameDomain.IsSomeoneWon(game);
            await Clients.Group(gameId.ToString()).GameFinish(winner);
        }

        await Clients.Group(gameId.ToString()).UpdateGame(new GameDto(game));

        //_gameUpdateProducer.ProduceGameUpdateCommand(game);
        _dbContext.Games.Update(game);
        await _dbContext.SaveChangesAsync();
    }
}