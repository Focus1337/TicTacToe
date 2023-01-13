using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Entities;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]"), OpenIdDictAuthorize]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public UserController(UserManager<User> userManager) =>
        _userManager = userManager;

    [HttpGet("Me")]
    public async Task<Guid> Me()
    {
        var identity = HttpContext.User.Identity;

        if (identity?.Name is null)
            throw new Exception("User is not logged in");

        return (await _userManager.FindByNameAsync(identity.Name) ?? throw new Exception("User name not found")).Id;
    }
}