using ChanBoardModernized.API.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChanBoardModernized.API.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(
        ChanContext context,
        IPasswordHasher<User> passwordHasher)
    {
        if (await context.Users.AnyAsync(u => u.Username == "admin"))
            return;

        var admin = new User
        {
            Id = Guid.Parse("B22698B8-44A2-4115-9631-1C2D1E2AC5F7"),
            Username = "admin",
            Role = Shared.Components.UserRole.Admin,
        };
        admin.PasswordHash = passwordHasher.HashPassword(admin, "adminpassword");


        var user = new User
        {
            Id = Guid.Parse("D1F5C8E2-3A4B-4F6D-9C2E-8B7A9D5E4C3B"),
            Username = "regularuser",
            Role = Shared.Components.UserRole.User,
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "userpassword");

        var moderator = new User
        {
            Id = Guid.Parse("A3E2F1C4-5B6D-7E8F-9A0B-1C2D3E4F5A6B"),
            Username = "moderator",
            Role = Shared.Components.UserRole.Moderator,
        };
        moderator.PasswordHash = passwordHasher.HashPassword(moderator, "modpassword");


        context.Users.Add(admin);
        context.Users.Add(user);
        context.Users.Add(moderator);
        await context.SaveChangesAsync();
    }
}

