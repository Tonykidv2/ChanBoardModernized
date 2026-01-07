using ChanBoardModernized.Shared.Components.DTOs;
using ChanBoardModernized.Shared.Components.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;

namespace ChanBoardModernized.Shared.Components.Implementations;

public class ChanBoardHttpClient : IChanBoardHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;

    public ChanBoardHttpClient(IHttpClientFactory factory, ITokenStore tokenStore)
    {
        _httpClient = factory.CreateClient("ChanAPIClient");
        _tokenStore = tokenStore;
    }

    public async Task<List<UserDTO>> GetAllUsersAsync()
    {
        var result = new List<UserDTO>();

        await AddAuthorizationHeaderAsync();

        result = await _httpClient.GetFromJsonAsync<List<UserDTO>>("api/users") 
            ?? new List<UserDTO>();

        return result;
    }

    public async Task<UserDTO?> GetUserByUsernameAsync(string username)
    {
        await AddAuthorizationHeaderAsync();

        var result = await _httpClient.GetFromJsonAsync<UserDTO?>($"api/users/{username}");
        return result;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDTO loginCred)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginCred);

            if (response.IsSuccessStatusCode)
            {
                var authResponse =
                    await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                if (authResponse is not null)
                {
                    if(!authResponse.hasError)
                        await _tokenStore.SaveTokenAsync(authResponse.Token);

                    return authResponse;
                }
                return new AuthResponseDto(string.Empty, "Invalid response from server");
            }
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new AuthResponseDto(string.Empty, errorMessage);
        }
        catch (Exception ex)
        {
            var errorMsg = $"An error occurred during login: {ex.Message}";
            return new AuthResponseDto(string.Empty, errorMsg);
        }
    }

    public Task<AuthResponseDto> RegisterAsync(RegisterDTO registerCred)
    {
        throw new NotImplementedException();
    }

    private async Task AddAuthorizationHeaderAsync()
    {
        var token = await _tokenStore.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
