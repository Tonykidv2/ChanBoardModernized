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

        app.MapPost("/api/auth/refresh", async (RefreshTokenRequest request, AuthService authService) =>
        {
            var result = await authService.RefreshTokenAsync(request.RefreshToken);
            if (result.hasError)
            {
                return Results.Unauthorized();
            }
            return Results.Ok(result);
        });

        app.MapPost("/api/auth/revoke", async (RefreshTokenRequest request, AuthService authService) =>
        {
            var success = await authService.RemoveRefreshToken(request.RefreshToken);
            if (!success)
            {
                return Results.BadRequest("Invalid token");
            }
            return Results.Ok("Token revoked");
        }).RequireAuthorization();

        return app;
    }
}

