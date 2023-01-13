using Microsoft.AspNetCore.Identity;

namespace Presentation.Entities;

public class User : IdentityUser
{
    public required string Username { get; set; }

    public required string Password { get; set; }
    
    public int Rating { get; set; }
}