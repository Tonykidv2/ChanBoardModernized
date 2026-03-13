using ChanBoardModernized.API.Storage;

namespace ChanBoardModernized.API.EndPoints;

public static class FilesEndPoints
{

    public static IEndpointRouteBuilder MapFileEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/files", async (IFormFile file, IBlobStorage blobStorage) =>
        {
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest("No file uploaded.");
            }
            using var stream = file.OpenReadStream();
            var contentType = file.ContentType;
            var fileId = await blobStorage.UploadAsync(stream, contentType);

            return Results.Ok(fileId);
        })
            .DisableAntiforgery()
            .WithTags("Files");


        app.MapGet("/files/{fieldId}", async (Guid fieldId, IBlobStorage blobStorage) =>
        {
            var file = await blobStorage.DownloadAsync(fieldId);
            if (file == null)
            {
                return Results.NotFound();
            }
            return Results.File(file.Stream, file.ContentType);
        })
            .WithTags("Files");

        app.MapDelete("/files/{fieldId}", async (Guid fieldId, IBlobStorage blobStorage) =>
        {
            await blobStorage.DeleteAsync(fieldId);

            return Results.NoContent();
        })
            .WithTags("Files");

        return app;
    }
}
