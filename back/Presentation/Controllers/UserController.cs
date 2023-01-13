using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Entities;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]"), OpenIdDictAuthorize]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserController(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // [HttpGet]
    // public async string Me()
    // {
    //     // 
    // }
}