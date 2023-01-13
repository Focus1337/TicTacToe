using Microsoft.AspNetCore.Identity;

namespace Presentation.Entities;

public class User : IdentityUser
{
    public required string Username { get; set; }

    public string Password { get; set; } = null!;

    public int Rating { get; set; }
}