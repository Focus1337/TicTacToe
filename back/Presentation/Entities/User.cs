using Microsoft.AspNetCore.Identity;

namespace Presentation.Entities;

public class User : IdentityUser<Guid>
{
    public int Rating { get; set; } = 0;
}