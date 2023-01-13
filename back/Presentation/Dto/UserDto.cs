using System.ComponentModel.DataAnnotations;

namespace Presentation.Dto;

public class UserDto
{
    [Required] 
    public required string Username { get; set; }

    [Required] 
    public required string Password { get; set; }
}