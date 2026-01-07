using ChanBoardModernized.Shared.Components.Interfaces;

namespace ChanBoardModernized.Web.Services;

public class WebTokenStore : ITokenStore
{
    private string? _token;

    public Task ClearTokenAsync()
    {
        _token = null;
        return Task.CompletedTask;
    }

    public Task<string?> GetTokenAsync()
    {
        return Task.FromResult(_token);
    }

    public Task SaveTokenAsync(string token)
    {
        _token = token;
        return Task.CompletedTask;
    }
}
