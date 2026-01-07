using ChanBoardModernized.Shared.Components.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanBoardModernized.Shared.Components.Interfaces;

public interface IChanBoardHttpClient
{
    Task<AuthResponseDto> LoginAsync(LoginDTO loginCred);

    Task<AuthResponseDto> RegisterAsync(RegisterDTO registerCred);

    //Task<bool> CheckUsernameAvailabilityAsync(string username);

    Task<List<UserDTO>> GetAllUsersAsync();

    Task<UserDTO?> GetUserByUsernameAsync(string username);
}
