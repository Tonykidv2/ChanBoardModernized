using MongoDB.Bson.Serialization.Attributes;

namespace ChanBoardModernized.API.Data.Entities;

public class RefreshToken
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
