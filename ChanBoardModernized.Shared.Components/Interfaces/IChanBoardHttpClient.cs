using ChanBoardModernized.Shared.Components.DTOs;
using ChanBoardModernized.Shared.Components.DTOsl;

namespace ChanBoardModernized.Shared.Components.Interfaces;

public interface IChanBoardHttpClient
{
    Task<AuthResponseDto> LoginAsync(LoginDTO loginCred);

    Task<AuthResponseDto> RegisterAsync(RegisterDTO registerCred);

    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);

    //Task<bool> CheckUsernameAvailabilityAsync(string username);

    Task<List<UserDTO>> GetAllUsersAsync();

    Task<UserDTO?> GetUserByUsernameAsync(string username);

    Task<List<BoardDTO>> GetBoards();

    Task<BoardResponseDTO> CreateBoard(BoardDTO boardDto);

    Task<List<ThreadDTO>> GetPreviewThreads(string boardShortName, int pageNumber, int pageSize);

    Task<ThreadResponseDTO> CreateThread(ThreadDTO threadDto);

    Task<CommentResponseDTO> CreateComment(CommentDTO commentDto);

    Task<List<CommentDTO>> GetCommentsForThread(Guid threadId);
}
