using ChanBoardModernized.Shared.Components.DTOs;
using ChanBoardModernized.Shared.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ChanBoardModernized.Shared.Services;

public class TokenRefreshHandler : DelegatingHandler
{
    private readonly ITokenStore _tokenStore;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public TokenRefreshHandler(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Add auth header to request
        var token = await _tokenStore.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // If 401 Unauthorized, try to refresh token once
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _refreshLock.WaitAsync(cancellationToken);
            try
            {
                var refreshToken = await _tokenStore.GetRefreshTokenAsync();
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return response; // No refresh token available
                }

                // Call refresh endpoint
                var refreshResponse = await RefreshTokenAsync(refreshToken, cancellationToken);
                if (refreshResponse != null && !refreshResponse.hasError)
                {
                    await _tokenStore.SaveTokenAsync(refreshResponse.Token);
                    await _tokenStore.SaveRefreshTokenAsync(refreshResponse.RefreshToken);

                    // Retry the original request with new token
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshResponse.Token);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        return response;
    }

    private async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        // Create a new request to refresh endpoint
        using var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh")
        {
            Content = JsonContent.Create(new RefreshTokenRequest { RefreshToken = refreshToken })
        };

        //try 3 times to refresh token in case of transient errors
        int maxRetries = 3;
        for (int retry = 0; retry < maxRetries; retry++)
        {
            try
            {
                var response = await base.SendAsync(refreshRequest, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthResponseDto>(cancellationToken: cancellationToken);
                }
                else if ((int)response.StatusCode >= 500)
                {
                    // Transient server error, wait and retry
                    await Task.Delay(1000 * (retry + 1), cancellationToken);
                }
                else
                {
                    // Non-transient error, stop retrying
                    break;
                }
            }
            catch (HttpRequestException)
            {
                // Network error, wait and retry
                await Task.Delay(1000 * (retry + 1), cancellationToken);
            }
        }

        return null;
    }
}
