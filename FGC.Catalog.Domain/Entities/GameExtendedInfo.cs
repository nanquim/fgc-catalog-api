using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FGC.Catalog.Domain.Entities;

public class GameExtendedInfo
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    // Referência ao Game no PostgreSQL
    [BsonRepresentation(BsonType.String)]
    public Guid GameId { get; set; }

    public List<string> Screenshots { get; set; } = [];
    public List<string> Tags { get; set; } = [];
    public List<string> Platforms { get; set; } = [];
    public double AverageRating { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
}
