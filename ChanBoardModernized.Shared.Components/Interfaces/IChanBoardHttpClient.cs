using ChanBoardModernized.Shared.Components.DTOs;

namespace ChanBoardModernized.Shared.Components.Interfaces;

public interface IChanBoardHttpClient
{
    Task<AuthResponseDto> LoginAsync(LoginDTO loginCred);

    Task<AuthResponseDto> RegisterAsync(RegisterDTO registerCred);

    //Task<bool> CheckUsernameAvailabilityAsync(string username);

    Task<List<UserDTO>> GetAllUsersAsync();

    Task<UserDTO?> GetUserByUsernameAsync(string username);

    Task<List<BoardDTO>> GetBoards();

    Task<BoardResponseDTO> CreateBoard(BoardDTO boardDto);
}
