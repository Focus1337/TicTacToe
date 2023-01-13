using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Context;
using Presentation.Dto;
using Presentation.Entities;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class TicTacController : ControllerBase
{
    private readonly PostgresDbContext _postgresDbContext;

    public TicTacController(PostgresDbContext postgresDbContext)
    {
        _postgresDbContext = postgresDbContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame()
    {
        var game = new Game();
        await _postgresDbContext.Games.AddAsync(game);
        await _postgresDbContext.SaveChangesAsync();
        return Ok(game.Id);
    }

    public record GetGamesDto(int Page);

    [HttpGet, OpenIdDictAuthorize]
    public async Task<IActionResult> GetGames([FromQuery]GetGamesDto getGamesDto)
    {
        const int pageLength = 5;
        return Ok(await _postgresDbContext.Games
            .Where(g => g.Status != GameStatus.Finished)
            .OrderBy(g => g.Status)
            .Skip(getGamesDto.Page * pageLength)
            .Take(pageLength)
            .Select(g => new GameDto(g))
            .ToListAsync());
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
         _postgresDbContext.Games.RemoveRange(_postgresDbContext.Games);
         await _postgresDbContext.SaveChangesAsync();
         return Ok();
    }
}