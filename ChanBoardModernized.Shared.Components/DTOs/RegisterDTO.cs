using System.ComponentModel.DataAnnotations;

namespace ChanBoardModernized.Shared.Components.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
        
        [Required]
        public string Email { get; set; } = null!;
    }
}
