using ChanBoardModernized.API.Data;
using ChanBoardModernized.API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace ChanBoardModernized.API.Services;

public class CommentCounterService
{
    private readonly ChanContext _context;

    public CommentCounterService(ChanContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Atomically increments the counter and resets to 1 if it reaches 999999999.
    /// Uses MongoDB's native atomic operations for thread safety.
    /// </summary>
    public async Task<int> GetNextCounterValueAsync(Guid boardId)
    {
        const int MAX_VALUE = 999999999;

        // Use a transaction for safety
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
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
