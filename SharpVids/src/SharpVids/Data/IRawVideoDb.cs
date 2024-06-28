using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace SharpVids.Data;

/// <summary>
/// A wrapper for interfacing with our raw video MongoDB persistance
/// </summary>
public interface IRawVideoDb
{
    public IMongoCollection<T> GetCollection<T>(string dbName, string collectionName);
    public IGridFSBucket GetBucketFromDb(string dbName);
}
