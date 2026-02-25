using ChanBoardModernized.Shared.Components.Interfaces;

namespace ChanBoardModernized.Web.Client.Services;

public class WebTokenStore : ITokenStore
{
    private string? _token;
    private const string RefreshTokenKey = "auth_refresh_jwt";

    public Task ClearRefreshTokenAsync()
    {
        throw new NotImplementedException();
    }

    public Task ClearTokenAsync()
    {
        _token = null;
        return Task.CompletedTask;
    }

    public Task<string?> GetRefreshTokenAsync()
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetTokenAsync()
    {
        return Task.FromResult(_token);
    }

    public Task SaveRefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task SaveTokenAsync(string token)
    {
        _token = token;
        return Task.CompletedTask;
    }
}
