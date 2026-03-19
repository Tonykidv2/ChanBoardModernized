using System.ComponentModel.DataAnnotations;

namespace ChanBoardModernized.API.Data.Entities;

public class Thread
{
    [Key]
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "Anonymous";
    public Guid CreatedByUserId { get; set; }
    public Guid BoardId { get; set; }
    public int CommentCount { get; set; }
  
    public Board Board { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
