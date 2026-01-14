using ChanBoardModernized.Shared.Components.DTOs;
using ChanBoardModernized.Shared.Components.DTOsl;
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
    
    private async Task AddAuthorizationHeaderAsync()
    {
        var token = await _tokenStore.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
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

    public async Task<List<BoardDTO>> GetBoards()
    {
        var result = new List<BoardDTO>();
        await AddAuthorizationHeaderAsync();
        result = await _httpClient.GetFromJsonAsync<List<BoardDTO>>("api/boards")
            ?? new List<BoardDTO>();
        return result;
    }

    public async Task<BoardResponseDTO> CreateBoard(BoardDTO boardDto)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var response = await _httpClient.PostAsJsonAsync("api/boards", boardDto);
            if (response.IsSuccessStatusCode)
            {
                var createResponse =
                    await response.Content.ReadFromJsonAsync<BoardResponseDTO>();

                if (createResponse != null)
                {
                    return createResponse;
                }

                return new BoardResponseDTO(null, "Invalid response from server");
            }
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new BoardResponseDTO(null, errorMessage);
        }
        catch(Exception ex)
        {
            var errorMsg = $"An error occurred: {ex.Message}";
            return new BoardResponseDTO(null, errorMsg);
        }

    }

    public async Task<List<ThreadDTO>> GetPreviewThreads(string boardShortName, int pageNumber, int pageSize)
    {
       await AddAuthorizationHeaderAsync();
        var result = await _httpClient.GetFromJsonAsync<List<ThreadDTO>>($"api/boards/{boardShortName}/threads/{pageNumber}/{pageSize}")
            ?? new List<ThreadDTO>();
        return result;
    }

    public async Task<ThreadResponseDTO> CreateThread(ThreadDTO threadDto)
    {
        await AddAuthorizationHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync("api/boards/threads", threadDto);

        if (response.IsSuccessStatusCode)
        {
            var createdThread = await response.Content.ReadFromJsonAsync<ThreadResponseDTO>();
            if (createdThread != null)
            {
                return createdThread;
            }
            return new ThreadResponseDTO(null, "Invalid response from server");
        }
        var errorMessage = await response.Content.ReadAsStringAsync();
        return new ThreadResponseDTO(null, errorMessage);
    }

    public async Task<CommentResponseDTO> CreateComment(CommentDTO commentDto)
    {
        await AddAuthorizationHeaderAsync();
        throw new NotImplementedException();
    }

    public async Task<List<CommentDTO>> GetCommentsForThread(Guid threadId)
    {
        await AddAuthorizationHeaderAsync();
        var result = await _httpClient.GetFromJsonAsync<List<CommentDTO>>($"api/threads/{threadId}/comments")
            ?? new List<CommentDTO>();
        return result;
    }
}
