using ChanBoardModernized.Shared.Components.DTOs;
using System.Security.Claims;

namespace ChanBoardModernized.Shared.Components.Interfaces;

public interface IAuthState
{
    ClaimsPrincipal? User { get; }
    string? UserName { get; }
    UserDTO UserDto { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(UserRole role);
    Task LoadAsync();
    Task LogoutAsync();
}
