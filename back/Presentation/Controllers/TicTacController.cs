using Microsoft.AspNetCore.Mvc;
using Presentation.Data;
using Presentation.Entities;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class TicTacController : ControllerBase
{
    private readonly MongoDbContext _dbContext;

    public TicTacController(MongoDbContext dbContext) =>
        _dbContext = dbContext;

    [HttpPost]
    public async Task<IActionResult> CreateGame()
    {
        var game = Game.CreateNew();
        await _dbContext.GetGameCollection().InsertOneAsync(game);
        return Ok(game.Id);
    }
}