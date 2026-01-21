using ChanBoardModernized.API.Data;
using ChanBoardModernized.API.Data.Entities;
using ChanBoardModernized.Shared.Components.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

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
        
        var token = GenerateJwtToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);
        return new AuthResponseDto(token, string.Empty) { RefreshToken = refreshToken };
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
        
        var token = GenerateJwtToken(newUser);
        var refreshToken = await GenerateRefreshTokenAsync(newUser.Id);
        return new AuthResponseDto(token, string.Empty) { RefreshToken = refreshToken };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            return new AuthResponseDto(string.Empty, "Invalid or expired refresh token");
        }

        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == storedToken.UserId);

        if (user == null)
        {
            return new AuthResponseDto(string.Empty, "User not found");
        }

        // Revoke old refresh token and generate new ones
        storedToken.IsRevoked = true;
        var newRefreshToken = await GenerateRefreshTokenAsync(user.Id);
        storedToken.ReplacedByToken = newRefreshToken;

        _dbContext.RefreshTokens.Update(storedToken);
        await _dbContext.SaveChangesAsync();

        var newAccessToken = GenerateJwtToken(user);

        return new AuthResponseDto(newAccessToken, string.Empty) { RefreshToken = newRefreshToken };
    }

    public async Task RevokeAllUserTokensAsync(Guid userId)
    {
        var userTokens = await _dbContext.RefreshTokens
        .Where(rt => rt.UserId == userId && !rt.IsRevoked)
        .ToListAsync();

        foreach (var token in userTokens)
        {
            token.IsRevoked = true;
        }

        _dbContext.RefreshTokens.UpdateRange(userTokens);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> RemoveRefreshToken(string refreshToken) 
    {
        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null || storedToken.IsRevoked)
        {
            return false;
        }

        storedToken.IsRevoked = true;
        _dbContext.RefreshTokens.Update(storedToken);
        await _dbContext.SaveChangesAsync();

        return true;
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
        var secrets = _configuration.GetValue<string>("JWT:secret"); // get from configuration/appsettings
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

    private async Task<string> GenerateRefreshTokenAsync(Guid userId)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var token = Convert.ToBase64String(randomBytes);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Longer-lived refresh token
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        return token;
    }
}
