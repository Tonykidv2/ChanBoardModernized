using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChanBoardModernized.API.Data.Entities;

public class Photo
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(255)]
    public string BlobPath { get; set; } = null!;
    public string OriginalFileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSizeBytes { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }

    public DateTime UploadedAt { get; set; }
    public Guid UserId { get; set; }
}
