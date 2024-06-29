using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace SharpVids.Data;

/// <inheritdoc cref="IRawVideoDb"/>
public sealed class RawVideoDb : IRawVideoDb
{
    public RawVideoDb(IMongoDbConnectionFactory connectionFactory)
    {
        _client = connectionFactory.Create();
    }

    /// <inheritdoc />
    public IMongoCollection<T> GetCollection<T>(string dbName, string collectionName)
    {
        var db = GetDatabase(dbName);
        return db.GetCollection<T>(collectionName);
    }

    /// <inheritdoc />
    public IGridFSBucket GetBucketFromDb(string dbName)
    {
        var db = GetDatabase(dbName);
        return new GridFSBucket(db);
    }

    private IMongoDatabase GetDatabase(string dbName)
    {
        return _client.GetDatabase(dbName);
    }

    private readonly MongoClientBase _client;
}
