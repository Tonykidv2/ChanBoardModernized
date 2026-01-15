using MongoDB.Bson.Serialization.Attributes;

namespace ChanBoardModernized.API.Data.Entities;

public class CommentCounter
{
    [BsonId]
    public Guid Id { get; set; }
    public Guid BoardId { get; set; }
    public int Value { get; set; } = 0;
}
