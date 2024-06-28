using MongoDB.Driver;

namespace SharpVids.Data;

/// <summary>
/// A wrapper for connecting to our MongoDB service to be used as a raw video database.
/// </summary>
public interface IRawVideoDb
{
    public IMongoClient Client { get; }
}
