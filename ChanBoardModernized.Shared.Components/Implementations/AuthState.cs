using ChanBoardModernized.Shared.Components.DTOs;
using ChanBoardModernized.Shared.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChanBoardModernized.Shared.Components.Implementations;

public class AuthState : IAuthState
{
    private readonly ITokenStore _tokenStore;

    public ClaimsPrincipal? User { get; private set; }

    public string? UserName => User?.Identity?.Name;

    public UserDTO UserDto { get; private set; }

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public AuthState(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    public async Task LoadAsync()
    {
        var token = await _tokenStore.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return;

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        if (jwt.ValidTo < DateTime.UtcNow)
        {
            await LogoutAsync();
            return;
        }

        User = new ClaimsPrincipal(
            new ClaimsIdentity(jwt.Claims, "jwt"));

        UserDto = new UserDTO
        {
            Username = string.IsNullOrEmpty(User?.Identity?.Name) ? "Guest" : User.Identity.Name,
            Email = User?.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty,
            Role = Enum.TryParse<UserRole>(
                User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                out var role) ? role : UserRole.Guest
        };
    }

    public bool IsInRole(UserRole role) =>
        User?.IsInRole(role.ToString()) == true;

    public async Task LogoutAsync()
    {
        await _tokenStore.ClearTokenAsync();
        User = null;
    }
}
