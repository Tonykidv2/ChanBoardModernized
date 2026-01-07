using ChanBoardModernized.API.Data;
using ChanBoardModernized.API.Data.Entities;
using ChanBoardModernized.Shared.Components.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChanBoardModernized.API.Services;

public class AuthService
{
    private readonly ChanContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(ChanContext dbContext, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDTO loginCred)
    {
        // Placeholder for actual authentication logic
        var user = await _dbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Username == loginCred.Username);
        
        if (user == null)
        {
            // User not found
            return new AuthResponseDto(string.Empty, "User not found");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginCred.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            // Invalid password
            return new AuthResponseDto(string.Empty, "Invalid password");
        }
        // Successful login logic here (e.g., generate JWT token)
        var token = GenerateJwtToken(user);
        return new AuthResponseDto(token, string.Empty);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDTO registerCred)
    {
        // Check if user already exists
        var existingUser = await _dbContext
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Username == registerCred.Username);
        
        if (existingUser != null)
        {
            // User already exists
            return new AuthResponseDto(string.Empty, "User already exists");
        }
        // Create new user
        var newUser = new User
        {
            Username = registerCred.Username,
            Role = Shared.Components.UserRole.User// Default role
        };
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, registerCred.Password);
        if (!string.IsNullOrEmpty(registerCred.Email))
        {
            newUser.Email = registerCred.Email;
        }
        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();
        // Successful registration logic here (e.g., generate JWT token)
        var token = GenerateJwtToken(newUser);
        return new AuthResponseDto(token, string.Empty);
    }

    private string GenerateJwtToken(User user)
    {
        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("email", user.Email ?? string.Empty)
        };
        var secrets = _configuration.GetValue<string>("JWT:secret"); // Placeholder for secret key retrieval.. get from configuration/appsettings
        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secrets));

        var sighingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("JWT:issuer"), 
            audience: _configuration.GetValue<string>("JWT:audience"), 
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_configuration.GetValue<int>("JWT:ExpirationInHours")),
            signingCredentials: sighingCredentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        // Successfully generated JWT token
        return token;
    }
}
