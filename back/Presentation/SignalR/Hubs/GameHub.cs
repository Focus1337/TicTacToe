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

    public GameHub(GameUpdateProducer gameUpdateProducer, PostgresDbContext dbContext)
    {
        _gameUpdateProducer = gameUpdateProducer;
        _dbContext = dbContext;
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

            if (game is { Status: GameStatus.New, PlayerX: { }, PlayerO: { } })
                game.Status = GameStatus.Started;

            _dbContext.Games.Update(game);
            await _dbContext.SaveChangesAsync();
        }

        if (game is { } && (game.PlayerX == userId || game.PlayerO == userId))
        {
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

    public async Task PlaceFigure(int first, int second, Guid gameId, Guid userId)
    {
        var game = await _dbContext.Games.FirstOrDefaultAsync(game => game.Id == gameId);

        Console.WriteLine(game[first, second]);

        if (game is null or { Status: GameStatus.Finished } || game[first, second] != Figure.None) return;

        var whoseMove = GameDomain.WhoseMove(game);
        Console.WriteLine(whoseMove);
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

        Console.WriteLine(whoseMove);

        game[first, second] = whoseMove;
        game.Status = GameStatus.Started;

        if (GameDomain.IsGameFinished(game))
        {
            game.Status = GameStatus.Finished;
            var winnerFigure = GameDomain.IsSomeoneWon(game);
            await Clients.Group(gameId.ToString()).GameFinish(winnerFigure);
            if (winnerFigure != Figure.None)
            {
                var winner = await _dbContext.Users
                    .FirstAsync(u => u.Id == (winnerFigure == Figure.X ? game.PlayerX : game.PlayerO));
                var looser = await _dbContext.Users
                    .FirstAsync(u => u.Id == (winnerFigure == Figure.X ? game.PlayerO : game.PlayerX));
                winner.Rating += 3;
                looser.Rating -= 1;
            }
        }

        await Clients.Group(gameId.ToString()).UpdateGame(new GameDto(game));

        //_gameUpdateProducer.ProduceGameUpdateCommand(game);
        _dbContext.Games.Update(game);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Restart(Guid gameId)
    {
        var game = await _dbContext.Games.FirstOrDefaultAsync(game => game.Id == gameId);
        if (game is { Status: GameStatus.Finished })
        {
            game.Restart();

            await Clients.Group(gameId.ToString()).UpdateGame(new GameDto(game));
            
            _dbContext.Games.Update(game);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task GoBack(Guid gameId, Guid userId)
    {
        
    }
}