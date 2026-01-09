using ChanBoardModernized.API.Data;
using ChanBoardModernized.API.Data.Entities;
using ChanBoardModernized.Shared.Components;
using ChanBoardModernized.Shared.Components.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ChanBoardModernized.API.EndPoints;

public static class UserEndPoints
{
    public static IEndpointRouteBuilder MapUserEndPoints(this IEndpointRouteBuilder app)
    {
        //Get user by name
        app.MapGet("/api/users/{username}", async (string username, ChanContext dbContext) =>
        {
            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(new
            {
                user.Id,
                user.Username,
                user.Role
            });

        }).RequireAuthorization(policy => 
            policy.RequireRole(
                UserRole.Admin.ToString(), 
                UserRole.Moderator.ToString(), 
                UserRole.User.ToString()));

        //get all users (admin only)
        app.MapGet("/api/users", async (ChanContext dbContext) =>
        {
            var users = await dbContext.Users
                .AsNoTracking()
                .Select(u => new UserDTO()
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email ?? string.Empty,
                    Role = u.Role
                })
                .ToListAsync();
            return Results.Ok(users);
        }).RequireAuthorization(policy => 
            policy.RequireRole(
                UserRole.Admin.ToString()));

        //register new user
        app.MapPost("/api/users/register", async (RegisterDTO registerDto, ChanContext dbContext) =>
        {
            // Check if user already exists
            var existingUser = await dbContext
                .Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Username == registerDto.Username);
            if (existingUser != null)
            {
                return Results.Conflict("User already exists");
            }
            // Create new user
            var newUser = new User
            {
                Username = registerDto.Username,
                Role = UserRole.User // Default role
            };
            newUser.PasswordHash = dbContext.PasswordHasher.HashPassword(newUser, registerDto.Password);
            if (!string.IsNullOrEmpty(registerDto.Email))
            {
                newUser.Email = registerDto.Email;
            }
            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync();
            return Results.Created();
        });

        return app;
    }
}
