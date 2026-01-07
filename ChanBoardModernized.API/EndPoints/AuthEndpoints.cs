using ChanBoardModernized.API.Services;
using ChanBoardModernized.Shared.Components.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ChanBoardModernized.API.EndPoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", [AllowAnonymous] async (LoginDTO loginDto, AuthService authService) =>
        {
            return Results.Ok(await authService.LoginAsync(loginDto));
        });

        app.MapGet("/api/auth/test", [Authorize]() =>
        {
            return Results.Ok("You are authorized to access this endpoint.");
        });
        return app;
    }


}
