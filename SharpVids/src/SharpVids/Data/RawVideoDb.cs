using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace SharpVids.Data;

/// <inheritdoc cref="IRawVideoDb"/>
public sealed class RawVideoDb : IRawVideoDb
{
    public RawVideoDb(ILogger<RawVideoDb> logger, IConfiguration configuration)
    {
        _logger = logger;
        _client = TryToConnectToRawVideoDb(configuration);
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

    private MongoClient TryToConnectToRawVideoDb(IConfiguration configuration)
    {
        var connectionString = TryToGetRawVideoDbConnectionString(configuration);

        try
        {
            return new MongoClient(connectionString);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to connect to raw video db.");
            throw;
        }
    }

    private string TryToGetRawVideoDbConnectionString(IConfiguration configuration)
    {
        const string RAW_VIDEO_DB_CONNECTION_STRING_NAME = "raw-video-db";
        var connectionString = configuration.GetConnectionString(RAW_VIDEO_DB_CONNECTION_STRING_NAME);
        if (connectionString is null)
        {
            var ex = new InvalidOperationException($"Unable to find connection string '{RAW_VIDEO_DB_CONNECTION_STRING_NAME}' for raw video db.");
            _logger.LogCritical(ex, "Failed to connect to raw video db.");
            throw ex;
        }

        return connectionString;
    }

    private readonly ILogger<RawVideoDb> _logger;
    private readonly MongoClientBase _client;
}

public static class RawVideoDbInstaller
{
    public static IServiceCollection AddRawVideoDb(this IServiceCollection services)
    {
        services.AddTransient<IRawVideoDb, RawVideoDb>();

        return services;
    }
}
