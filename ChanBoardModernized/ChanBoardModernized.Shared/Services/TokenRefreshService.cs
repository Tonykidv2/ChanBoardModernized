using ChanBoardModernized.Shared.Components.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace ChanBoardModernized.Shared.Services;

public class TokenRefreshService : IDisposable
{

    private readonly ITokenStore _tokenStore;
    private readonly IChanBoardHttpClient _chanClient;
    private Timer? _refreshTimer;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    public TokenRefreshService(ITokenStore tokenStore, IChanBoardHttpClient chanClient)
    {
        _tokenStore = tokenStore;
        _chanClient = chanClient;
    }

    public async Task StartAsync()
    {
        await ScheduleNextRefreshAsync();
    }

    private async Task ScheduleNextRefreshAsync()
    {
        var token = await _tokenStore.GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return;

        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var expiresAt = jwtToken.ValidTo;

        // Refresh 5 minutes before expiration
        var refreshAt = expiresAt.AddMinutes(-5);
        var delay = refreshAt - DateTime.UtcNow;

        if (delay.TotalMilliseconds > 0)
        {
            _refreshTimer?.Dispose();
            _refreshTimer = new Timer(
                async _ => await RefreshTokenAsync(),
                null,
                delay,
                Timeout.InfiniteTimeSpan
            );
        }
        else
        {
            // Token already expired or about to expire, refresh immediately
            await RefreshTokenAsync();
        }
    }

    private async Task RefreshTokenAsync()
    {
        await _refreshLock.WaitAsync();
        try
        {
            var refreshToken = await _tokenStore.GetRefreshTokenAsync();
            if (string.IsNullOrEmpty(refreshToken))
                return;

            var result = await _chanClient.RefreshTokenAsync(refreshToken);
            if (!result.hasError)
            {
                await _tokenStore.SaveTokenAsync(result.Token);
                await _tokenStore.SaveRefreshTokenAsync(result.RefreshToken);
                await ScheduleNextRefreshAsync();
            }
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public void Dispose()
    {
        _refreshTimer?.Dispose();
        _refreshLock?.Dispose();
    }
}
