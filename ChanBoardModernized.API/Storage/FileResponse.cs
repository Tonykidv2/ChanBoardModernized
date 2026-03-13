namespace ChanBoardModernized.API.Storage;

public record FileResponse(Stream Stream, string ContentType, long Length);
