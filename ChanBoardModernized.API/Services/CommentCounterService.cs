using ChanBoardModernized.API.Data;
using ChanBoardModernized.API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace ChanBoardModernized.API.Services;

public class CommentCounterService
{
    private readonly ChanContext _context;
    private readonly bool _isRaspberryPi;

    public CommentCounterService(ChanContext context, IConfiguration configuration)
    {
        _context = context;
        var deploymentTarget = configuration.GetValue<string>("DEPLOYMENT_TARGET") ?? "server";
        _isRaspberryPi = deploymentTarget.Equals("pi", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Atomically increments the counter and resets to 1 if it reaches 999999999.
    /// Uses MongoDB's native atomic operations for thread safety.
    /// </summary>
    public async Task<int> GetNextCounterValueAsync(Guid boardId, int maxRetries = 3)
    {
        const int MAX_VALUE = 999999999;

        // Use a transaction for safety
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            if (_isRaspberryPi)
            {
                var counter = await _context.CommentCounters
                    .Where(c => c.BoardId == boardId)
                    .FirstOrDefaultAsync();
                if (counter == null)
                {
                    counter = new CommentCounter
                    {
                        Id = Guid.NewGuid(),
                        BoardId = boardId,
                        Value = 1
                    };
                    _context.Add(counter);
                }
                else if (counter.Value >= MAX_VALUE)
                {
                    counter.Value = 1;
                }
                else
                {
                    counter.Value++;
                }
                try
                {
                    await _context.SaveChangesAsync();
                    return counter.Value;
                }
                catch (DbUpdateConcurrencyException) when (attempt < maxRetries - 1)
                {
                    // Retry on concurrency conflict
                    await Task.Delay(TimeSpan.FromMilliseconds(50 * (attempt + 1)));
                    continue;
                }
            }
            else
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Get or create counter with row lock (FOR UPDATE)
                    var counter = await _context.CommentCounters
                        .Where(c => c.BoardId == boardId)
                        .FirstOrDefaultAsync();

                    if (counter == null)
                    {
                        // Initialize new counter
                        counter = new CommentCounter
                        {
                            Id = Guid.NewGuid(),
                            BoardId = boardId,
                            Value = 1
                        };
                        _context.Add(counter);
                    }
                    else if (counter.Value >= MAX_VALUE)
                    {
                        // Reset to 1
                        counter.Value = 1;
                    }
                    else
                    {
                        // Normal increment
                        counter.Value++;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return counter.Value;
                }
                catch (DbUpdateConcurrencyException) when (attempt < maxRetries - 1)
                {
                    // Retry on concurrency conflict
                    await Task.Delay(TimeSpan.FromMilliseconds(50 * (attempt + 1)));
                }
            }
        }

        throw new InvalidOperationException("Failed to get comment digits");
    }
}
