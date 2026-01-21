using ChanBoardModernized.API.Data;
using Microsoft.EntityFrameworkCore;

namespace ChanBoardModernized.API.Services;

public class RefreshTokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RefreshTokenCleanupService> _logger;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(24); // Run daily
    private readonly int _retentionDays = 7; // Keep revoked tokens for 7 days

    public RefreshTokenCleanupService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<RefreshTokenCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        _retentionDays = configuration.GetValue<int>("RefreshTokenCleanup:RetentionDays", 7);
        _cleanupInterval = TimeSpan.FromHours(
            configuration.GetValue<int>("RefreshTokenCleanup:CleanupIntervalHours", 24));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RefreshTokenCleanupService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredTokensAsync(stoppingToken);
                await Task.Delay(_cleanupInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Service is stopping
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token cleanup");
                // Wait before retrying to avoid tight error loops
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("RefreshTokenCleanupService stopped");
    }

    private async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChanContext>();

        var cutoffDate = DateTime.UtcNow.AddDays(-_retentionDays);

        // Delete tokens that are:
        // 1. Expired AND past retention period, OR
        // 2. Revoked AND past retention period
        var tokensToDelete = await dbContext.RefreshTokens
            .Where(rt =>
                (rt.ExpiresAt < cutoffDate) ||
                (rt.IsRevoked && rt.CreatedAt < cutoffDate))
            .ToListAsync(cancellationToken);

        if (tokensToDelete.Any())
        {
            dbContext.RefreshTokens.RemoveRange(tokensToDelete);
            var deletedCount = await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Cleaned up {Count} expired/revoked refresh tokens older than {Date}",
                deletedCount,
                cutoffDate);
        }
        else
        {
            _logger.LogInformation("No expired tokens to clean up");
        }
    }
}
