using ChanBoardModernized.Shared.Components.Interfaces;

namespace ChanBoardModernized.Services;

public class SecureTokenStore : ITokenStore
{
    private const string TokenKey = "auth_jwt";

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
}
