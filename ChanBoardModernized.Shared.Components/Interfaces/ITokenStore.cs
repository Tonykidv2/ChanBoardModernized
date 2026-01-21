using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanBoardModernized.Shared.Components.Interfaces;

public interface ITokenStore
{
    Task SaveTokenAsync(string token);
    Task<string?> GetTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task SaveRefreshTokenAsync(string refreshToken);
    Task ClearTokenAsync();
    Task ClearRefreshTokenAsync();
}
