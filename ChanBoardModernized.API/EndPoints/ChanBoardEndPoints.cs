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
        })
            .WithTags("CommonBoardsEndpoints");

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
        })
            .WithTags("CommonBoardsEndpoints");

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

            // Get threads with pagination
            var threads = await dbContext.Threads
                .AsNoTracking()
                .Where(t => t.BoardId == board.Id)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new ThreadDTO()
                {
                    Id = t.Id,
                    Title = t.Title ?? "",
                    CreatedDate = t.CreatedAt,
                    Comments = new List<CommentDTO>(),
                    BoardId = t.BoardId,
                    CommentCount = t.CommentCount
                })
                .ToListAsync();

            if (!threads.Any())
            {
                return Results.Ok(threads);
            }

            var threadIds = threads.Select(t => t.Id).ToList();

            // Single query to get all relevant comments for all threads
            var allComments = await dbContext.Comments
                .AsNoTracking()
                .Where(c => threadIds.Contains(c.ThreadId))
                .OrderBy(c => c.ThreadId)
                .ThenBy(c => c.CreatedAt)
                .Select(c => new CommentDTO()
                {
                    Id = c.Id,
                    Content = c.TextContent,
                    CreatedAt = c.CreatedAt,
                    ThreadId = c.ThreadId,
                    PostDigits = c.PostDigits,
                    Author = c.DisplayAuthor
                })
                .ToListAsync();

            // Group comments by thread for efficient lookup
            var commentsByThread = allComments
                .GroupBy(c => c.ThreadId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Populate comments for each thread
            foreach (var thread in threads)
            {
                if (!commentsByThread.TryGetValue(thread.Id, out var threadComments))
                {
                    continue;
                }

                List<CommentDTO> selectedComments;

                if (thread.CommentCount < 4)
                {
                    selectedComments = threadComments;
                }
                else
                {
                    // First comment + last 2 comments
                    selectedComments = new List<CommentDTO>
                    {
                        threadComments[0]
                    };
                    selectedComments.AddRange(threadComments.TakeLast(2));
                }

                if (selectedComments.Any())
                {
                    selectedComments[0].Title = thread.Title;
                }

                thread.Comments = selectedComments;
            }

            return Results.Ok(threads);
        })
            .WithTags("CommonBoardsEndpoints");

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
                CommentCount = 1
            };

            //When MongoDb has been initalized
            var Nextdigit = await commentCounterService.GetNextCounterValueAsync(board.Id);
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                TextContent = threadDto.Content,
                CreatedAt = DateTime.UtcNow,
                ThreadId = thread.Id,
                PostDigits = Nextdigit,
                DisplayAuthor = threadDto.Author
            };
            dbContext.Threads.Add(thread);
            dbContext.Comments.Add(comment);
            await dbContext.SaveChangesAsync();
            threadDto.Id = thread.Id;
            var result = new ThreadResponseDTO(threadDto, string.Empty);
            return Results.Ok(result);
        })
            .WithTags("CommonBoardsEndpoints");
        
        //Create comment
        app.MapPost("api/comment", async (CommentDTO commentDto, ChanContext dbContext, CommentCounterService commentCounterService) =>
        {
            var thread = await dbContext.Threads.FirstOrDefaultAsync(t => t.Id == commentDto.ThreadId);
            if (thread != null)
            {
                thread.CommentCount += 1;
            }
            else
            {
                return Results.NotFound("Thread not found or deleted");
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                TextContent = commentDto.Content,
                CreatedAt = DateTime.UtcNow,
                ThreadId = commentDto.ThreadId,
                DisplayAuthor = commentDto.Author
            };
            var Nextdigit = await commentCounterService.GetNextCounterValueAsync(thread.BoardId);
            comment.PostDigits = Nextdigit;

            dbContext.Threads.Update(thread);
            dbContext.Comments.Add(comment);
            
            await dbContext.SaveChangesAsync();

            commentDto.Id = comment.Id;

            var result = new CommentResponseDTO(commentDto, string.Empty);
            return Results.Ok(result);
        })
            .WithTags("CommonBoardsEndpoints");

        //Get comments for a thread
        app.MapGet("/api/threads/{threadId}/comments", async (Guid threadId, ChanContext dbContext) =>
        {
            var thread = await dbContext.Threads
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == threadId);

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
                    PostDigits = c.PostDigits,
                    Author = c.DisplayAuthor
                })
                .ToListAsync();

            if (comments.Any())
            {
                comments[0].Title = thread?.Title ?? "";
            }

            return Results.Ok(comments);
        })
            .WithTags("CommonBoardsEndpoints");


        ///Admin Endpoints go here for managing boards, threads, comments, users, etc.

        //create board
        app.MapPost("/api/boards", async (BoardDTO boardDto, ChanContext dbContext) =>
        {
            try
            {
                if (await dbContext.Boards.AnyAsync(b => b.ShortName == boardDto.Name))
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
            catch (Exception ex)
            {
                return Results.InternalServerError(new BoardResponseDTO(null, "Something went wrong adding board to database"));
            }
        }).RequireAuthorization(policy =>
                policy.RequireRole(UserRole.Admin.ToString()))
                .WithTags("AdministrativeEndpoints");

        app.MapPost("/api/auth/cleanup-tokens", async (ChanContext dbContext) =>
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-7);

            var tokensToDelete = await dbContext.RefreshTokens
                .Where(rt =>
                    (rt.ExpiresAt < cutoffDate) ||
                    (rt.IsRevoked && rt.CreatedAt < cutoffDate))
                .ToListAsync();

            dbContext.RefreshTokens.RemoveRange(tokensToDelete);
            var count = await dbContext.SaveChangesAsync();

            return Results.Ok(new { DeletedCount = count, CutoffDate = cutoffDate });
        }).RequireAuthorization(policy => 
                policy.RequireRole(UserRole.Admin.ToString()))
                .WithTags("AdministrativeEndpoints");

        return app;
    }
}
