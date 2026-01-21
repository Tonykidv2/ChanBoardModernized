using ChanBoardModernized.Shared.Components.Interfaces;

namespace ChanBoardModernized.Services;

public class SecureTokenStore : ITokenStore
{
    private const string TokenKey = "auth_jwt";
    private const string RefreshTokenKey = "auth_refresh_jwt";

    public async Task SaveTokenAsync(string token)
    {
        await SecureStorage.SetAsync(TokenKey, token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.GetAsync(TokenKey);
    }

    public Task ClearTokenAsync()
    {
        SecureStorage.Remove(TokenKey);
        return Task.CompletedTask;
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        return await SecureStorage.GetAsync(RefreshTokenKey);
    }

    public async Task SaveRefreshTokenAsync(string refreshToken)
    {
        await SecureStorage.SetAsync(RefreshTokenKey, refreshToken);
    }

    public Task ClearRefreshTokenAsync()
    {
        SecureStorage.Remove(RefreshTokenKey);
        return Task.CompletedTask;
    }
}
