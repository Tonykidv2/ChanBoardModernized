using System.ComponentModel.DataAnnotations;

namespace ChanBoardModernized.Shared.Components.DTOs;

public class LoginDTO
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
