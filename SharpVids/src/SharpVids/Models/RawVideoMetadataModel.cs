using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharpVids.Models;

public sealed class RawVideoMetadataModel
{
    [BsonId]
    public ObjectId Id { get; } = ObjectId.GenerateNewId();

    public required ObjectId VideoId { get; init; }
}
