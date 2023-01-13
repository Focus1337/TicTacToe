using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Context;
using Presentation.Entities;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]"), OpenIdDictAuthorize]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly PostgresDbContext _dbContext;

    public UserController(UserManager<User> userManager, PostgresDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    [HttpGet("Me")]
    public async Task<IActionResult> Me()
    {
        var identity = HttpContext.User.Identity;

        if (identity?.Name is null)
            throw new Exception("User is not logged in");

        var user = await _userManager.FindByNameAsync(identity.Name) ?? throw new Exception("User name not found");

        return Ok(new { user.Id, user.Rating, user.UserName });
    }

    [HttpGet("rating")]
    public async Task<IActionResult> Rating() =>
        Ok(await _dbContext.Users
            .OrderByDescending(u => u.Rating)
            .Select(u => new { u.UserName, u.Rating })
            .ToListAsync());
}