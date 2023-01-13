using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Context;
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

    [HttpGet]
    public async Task<IActionResult> GetGames()
    {
        return Ok(await _postgresDbContext.Games.ToListAsync());
    }
}