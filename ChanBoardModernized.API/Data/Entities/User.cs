using ChanBoardModernized.Shared.Components;
using MongoDB.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ChanBoardModernized.API.Data.Entities;

[Collection("users")]
public class User
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(25)]
    public string Username { get; set; } = null!;
    [MaxLength(100)]
    public string? Email { get; set; }
    [MaxLength(255)]
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.User;
}
