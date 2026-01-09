using ChanBoardModernized.API.Data;
using ChanBoardModernized.Shared.Components;
using ChanBoardModernized.Shared.Components.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ChanBoardModernized.API.EndPointsl;

public static class ChanBoardEndPoints
{
    public static IEndpointRouteBuilder MapChanBoardEndPoints(this IEndpointRouteBuilder app)
    {
        //create board
        app.MapPost("/api/boards", async (BoardDTO boardDto, ChanContext dbContext) =>
        {
            try
            {
                if(await dbContext.Boards.AnyAsync(b => b.ShortName == boardDto.Name))
                {
                    return Results.Conflict(new BoardResponseDTO(null, "Board already Created"));
                }

                var board = new Data.Entities.Board
                {
                    Id = Guid.NewGuid(),
                    Name = boardDto.Name,
                    ShortName = boardDto.ShortName,
                    Description = boardDto.Description
                };
                dbContext.Boards.Add(board);
                await dbContext.SaveChangesAsync();
                boardDto.Id = board.Id;
                var result = new BoardResponseDTO(boardDto, string.Empty);
                return Results.Created($"/api/boards/{board.ShortName}", result);
            }
            catch(Exception ex)
            {
                return Results.InternalServerError(new BoardResponseDTO(null, "Something went wrong adding board to database"));
            }
        }).RequireAuthorization(policy =>
                policy.RequireRole(UserRole.Admin.ToString()));

        //get all boards
        app.MapGet("/api/boards", async (ChanContext dbContext) =>
        {
            var boards = await dbContext.Boards
            .AsNoTracking()
            .Select(boards => new BoardDTO()
            {
                Id = boards.Id,
                Name = boards.Name,
                ShortName = boards.ShortName,
                Description = boards.Description
            }).ToListAsync();
            return Results.Ok(boards);
        });

        //get board by short name
        app.MapGet("/api/boards/{shortName}", async (string shortName, ChanContext dbContext) =>
        {
            var board = await dbContext.Boards
            .AsNoTracking()
            .Where(b => b.ShortName == shortName)
            .Select(b => new BoardDTO()
            {
                Id = b.Id,
                Name = b.Name,
                ShortName = b.ShortName,
                Description = b.Description
            }).FirstOrDefaultAsync();
            if (board == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(board);
        });

        //Get Threads and recent comments for each thread for a board and pagination
        app.MapGet("/api/boards/{shortName}/threads", async (string shortName, int pageNumber, int pageSize, ChanContext dbContext) =>
        {
            var board = await dbContext.Boards
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.ShortName == shortName);
            if (board == null)
            {
                return Results.NotFound();
            }
            var threads = await dbContext.CommentThreads
            .AsNoTracking()
            .Where(t => t.BoardId == board.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.CreatedAt,
                RecentComments = dbContext.Comments
                    .AsNoTracking()
                    .Where(c => c.ThreadId == t.Id)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(2)
                    .Select(c => new
                    {
                        c.Id,
                        c.TextContent,
                        c.CreatedAt
                    }).ToList()
            }).ToListAsync();
            return Results.Ok(threads);
        });

        return app;
    }
}
