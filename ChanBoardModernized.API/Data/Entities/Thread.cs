using MongoDB.Bson.Serialization.Attributes;

namespace ChanBoardModernized.API.Data.Entities;

public class Thread
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "Anonymous";
    public Guid CreatedByUserId { get; set; }
    public Guid BoardId { get; set; }
    public int CommentCount { get; set; }

    [BsonIgnore]    
    public Board Board { get; set; } = null!;
    [BsonIgnore]
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
