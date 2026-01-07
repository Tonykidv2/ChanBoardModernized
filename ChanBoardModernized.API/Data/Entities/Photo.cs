using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChanBoardModernized.API.Data.Entities;

public class Photo
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(255)]
    public string Url { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
    public Guid UserId { get; set; }

    // Navigation property
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}
