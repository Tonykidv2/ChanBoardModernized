using ChanBoardModernized.API.Data;
using ChanBoardModernized.API.Data.Entities;
using ChanBoardModernized.API.Services;
using ChanBoardModernized.Shared.Components;
using ChanBoardModernized.Shared.Components.DTOs;
using ChanBoardModernized.Shared.Components.DTOsl;
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
        app.MapGet("/api/boards/{shortName}/threads/{pageNumber}/{pageSize}", async (string shortName, int pageNumber, int pageSize, ChanContext dbContext) =>
        {
            var board = await dbContext.Boards
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.ShortName == shortName);
            if (board == null)
            {
                return Results.NotFound();
            }
            var threads = await dbContext.Threads
            .AsNoTracking()
            .Where(t => t.BoardId == board.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new ThreadDTO()
            {
                Id = t.Id,
                Title = t.Title ?? "",
                CreatedDate = t.CreatedAt,
                Comments = new List<CommentDTO>()
            }).ToListAsync();

            foreach (var thread in threads)
            {
                var recentComments = await dbContext.Comments
                .AsNoTracking()
                .Where(c => c.ThreadId == thread.Id)
                .OrderByDescending(c => c.CreatedAt)
                .Take(2)
                .Select(c => new CommentDTO()
                {
                    Id = c.Id,
                    Content = c.TextContent,
                    CreatedAt = c.CreatedAt,
                    ThreadId = c.ThreadId,
                    PostDigits = c.PostDigits
                }).ToListAsync();

                thread.Comments = recentComments.OrderBy(c => c.CreatedAt).ToList();
            }
            return Results.Ok(threads);
        });

        //Create thread
        app.MapPost("api/boards/threads", async (ThreadDTO threadDto, ChanContext dbContext, CommentCounterService commentCounterService) =>
        {

            var board = await dbContext.Boards
            .FirstOrDefaultAsync(b => b.Id == threadDto.BoardId);
            if (board == null)
            {
                return Results.NotFound();
            }
            var thread = new Data.Entities.Thread
            {
                Id = Guid.NewGuid(),
                BoardId = board.Id,
                Title = threadDto.Title,
                CreatedAt = DateTime.UtcNow,
            };

            //When MongoDb has been initalized
            var Nextdigit = await commentCounterService.GetNextCounterValueAsync(board.Id);
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                TextContent = threadDto.Content,
                CreatedAt = DateTime.UtcNow,
                ThreadId = thread.Id,
                PostDigits = Nextdigit
            };
            dbContext.Threads.Add(thread);
            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync();
            threadDto.Id = thread.Id;
            var result = new ThreadResponseDTO(threadDto, string.Empty);
            return Results.Ok(result);
        });//.RequireAuthorization(policy =>
           //policy.RequireRole(UserRole.User.ToString(), UserRole.Admin.ToString()));)
           //Create Comment

        //Create comment
        app.MapPost("api/comment", async (CommentDTO commentDto, ChanContext dbContext) =>
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                TextContent = commentDto.Content,
                CreatedAt = DateTime.UtcNow,
                ThreadId = commentDto.ThreadId,
            };
            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync();

            commentDto.Id = comment.Id;

            var result = new CommentResponseDTO(commentDto, string.Empty);
            return Results.Ok(result);
        });

        //Get comments for a thread
        app.MapGet("/api/threads/{threadId}/comments", async (Guid threadId, ChanContext dbContext) =>
        {
            var comments = await dbContext.Comments
                .AsNoTracking()
                .Where(c => c.ThreadId == threadId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentDTO
                {
                    Id = c.Id,
                    Content = c.TextContent,
                    CreatedAt = c.CreatedAt,
                    ThreadId = c.ThreadId,
                    PostDigits = c.PostDigits
                })
                .ToListAsync();

            return Results.Ok(comments);
        });

        return app;
    }
}
