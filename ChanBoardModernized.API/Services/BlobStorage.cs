using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ChanBoardModernized.API.Storage;

namespace ChanBoardModernized.API.Services;

public class BlobStorage : IBlobStorage
{

    private const string ContainerName = "images";

    public BlobStorage(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    private readonly BlobServiceClient _blobServiceClient;

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

        var blobClient = containerClient.GetBlobClient(id.ToString());

        return blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public Task<FileResponse> DownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

        var blobClient = containerClient.GetBlobClient(id.ToString());

        return blobClient.DownloadAsync(cancellationToken)
            .ContinueWith(task =>
            {
                var response = task.Result;
                return new FileResponse
                (
                    response.Value.Content,
                    response.Value.Details.ContentType,
                    response.Value.Details.ContentLength
                );
            }, cancellationToken); 
    }

    public async Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

        var fieldId = Guid.NewGuid();
        var blobClient = containerClient.GetBlobClient(fieldId.ToString());

        await blobClient.UploadAsync(
            stream, 
            new BlobHttpHeaders  { ContentType = contentType  }, 
            cancellationToken: cancellationToken);

        return fieldId;
    }
}
