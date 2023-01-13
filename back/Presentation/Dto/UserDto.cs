using System.ComponentModel.DataAnnotations;

namespace Presentation.Dto;

public class UserDto
{
    [Required] 
    public required string Username { get; set; }

    [Required] 
    public required string Password { get; set; }

    [Required]
    public required string RepeatPassword { get; set; }
}

public class LoginUserDto
{
    public string grant_type { get; set; }
    [Required] 
    public required string Username { get; set; }

    [Required] 
    public required string Password { get; set; }
}