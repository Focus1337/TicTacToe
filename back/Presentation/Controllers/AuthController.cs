using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Presentation.Dto;
using Presentation.Entities;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(UserDto userDto)
    {
        if (!ModelState.IsValid) return BadRequest();

        var user = new User
        {
            Username = userDto.Username,
        };

        if (userDto.Password != userDto.RepeatPassword) 
            return BadRequest();
        
        var result = await _userManager.CreateAsync(user, userDto.Password);
        return result.Succeeded
            ? Ok()
            : Forbid(new AuthenticationProperties(result.Errors.ToDictionary(error => error.Code,
                error => error.Description)!));
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [HttpPost("Login"), Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Login([FromForm] UserDto userDto)
    {
        if (!ModelState.IsValid) return BadRequest();
        var request = HttpContext.GetOpenIddictServerRequest() ?? throw new Exception("OpenIdDict config is wrong");
        if (await _userManager.FindByNameAsync(userDto.Username) is not { } user)
            return Forbid(new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "the username is incorrect"
            }), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        if (!(await _signInManager.CheckPasswordSignInAsync(user, userDto.Password, lockoutOnFailure: true)).Succeeded)
            return Forbid(new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "the password is incorrect"
            }), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
        userPrincipal.SetScopes(new[]
        {
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles
        }.Intersect(request.GetScopes()));
        foreach (var claim in userPrincipal.Claims)
            claim.SetDestinations(GetDestinations(claim, userPrincipal));
        return SignIn(userPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
        {
            switch (claim.Type)
            {
                case OpenIddictConstants.Claims.Name:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;
                case OpenIddictConstants.Claims.Email:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Email))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;
                case OpenIddictConstants.Claims.Role:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Roles))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;
                case "AspNet.Identity.SecurityStamp":
                    yield break;
                default:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    yield break;
            }
        }
    }
}