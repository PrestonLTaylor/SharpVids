using MongoDB.Driver;

namespace SharpVids.Data;

/// <summary>
/// Used to create <see cref="MongoClientBase"/> instances by connecting to a MongoDB service.
/// </summary>
public interface IMongoDbConnectionFactory
{
    /// <summary>
    /// Creates a connection to a MongoDB service.
    /// </summary>
    /// <returns>The <see cref="MongoClientBase"/> created by connecting to the MongoDB service.</returns>
    public MongoClientBase Create();
}
