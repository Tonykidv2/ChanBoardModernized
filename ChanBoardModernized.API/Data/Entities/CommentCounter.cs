using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ChanBoardModernized.API.Data.Entities;

public class CommentCounter
{
    [BsonId]
    public Guid Id { get; set; }
    public Guid BoardId { get; set; }
    public int Value { get; set; } = 0;

    [Timestamp] // EF Core concurrency token
    public byte[]? Version { get; set; }
}
