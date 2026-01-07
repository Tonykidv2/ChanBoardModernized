using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChanBoardModernized.API.Data.Entities;

public class CommentThread
{
    [Key]
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedByUserId { get; set; }
    public Guid BoardId { get; set; }

    [ForeignKey(nameof(BoardId))]
    public virtual Board Board { get; set; } = null!;

    [ForeignKey(nameof(CreatedByUserId))]
    public virtual User CreatedByUser { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
