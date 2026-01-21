using ChanBoardModernized.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChanBoardModernized.API.Middleware;

public class RoleValidationMiddleware
{
    private readonly RequestDelegate _next;

    public RoleValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ChanContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tokenRole = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                var user = await dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                // If user deleted or role changed, reject the token
                if (user == null || user.Role.ToString() != tokenRole)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Your role has been changed. Please log in again."
                    });
                    return;
                }
            }
        }

        await _next(context);
    }
}

// Extension method for easy registration
public static class RoleValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseRoleValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RoleValidationMiddleware>();
    }
}
