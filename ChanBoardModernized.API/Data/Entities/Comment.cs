using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChanBoardModernized.API.Data.Entities;

public class Comment
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(500)]
    public string TextContent { get; set; } = null!;
    public int PostDigits { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CommentPhotoId { get; set; }
    public Guid UserId { get; set; }
    public Guid ThreadId { get; set; }

    [ForeignKey(nameof(CommentPhotoId))]
    public virtual Photo CommentPhoto { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;

    [ForeignKey(nameof(ThreadId))]
    public virtual CommentThread Thread { get; set; } = null!;
}
