using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Context;
using Presentation.Dto;
using Presentation.Entities;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]"), OpenIdDictAuthorize]
public class TicTacController : ControllerBase
{
    private readonly PostgresDbContext _postgresDbContext;
    private readonly UserManager<User> _userManager;

    public TicTacController(PostgresDbContext postgresDbContext, UserManager<User> userManager)
    {
        _postgresDbContext = postgresDbContext;
        _userManager = userManager;
    }

    public record CreateGameDto(int MaxRating);

    [HttpPost, OpenIdDictAuthorize]
    public async Task<IActionResult> CreateGame(CreateGameDto createGameDto)
    {
        var user = await _userManager.GetUserAsync(User);
        var game = new Game
        {
            MaxRating = createGameDto.MaxRating,
            CreatorName = user!.UserName,
        };
        await _postgresDbContext.Games.AddAsync(game);
        await _postgresDbContext.SaveChangesAsync();
        return Ok(game.Id);
    }

    public record GetGamesDto(int Page);

    public record GamesListDto(List<GameDto> Games, bool HasMore);

    [HttpGet, OpenIdDictAuthorize]
    public async Task<IActionResult> GetGames([FromQuery] GetGamesDto getGamesDto)
    {
        const int pageLength = 5;
        var games = await _postgresDbContext.Games
            .Where(g => g.Status != GameStatus.Finished)
            .OrderBy(g => g.Status)
            .ThenByDescending(g => g.CreatedDateTime)
            .Skip(getGamesDto.Page * pageLength)
            .Take(pageLength)
            .Select(g => new GameDto(g))
            .ToListAsync();
        var hasMore = await _postgresDbContext.Games.CountAsync() > pageLength * (getGamesDto.Page + 1);
        return Ok(new GamesListDto(games, hasMore));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
        _postgresDbContext.Games.RemoveRange(_postgresDbContext.Games);
        await _postgresDbContext.SaveChangesAsync();
        return Ok();
    }
}