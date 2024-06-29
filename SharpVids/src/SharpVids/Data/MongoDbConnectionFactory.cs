using MongoDB.Driver;

namespace SharpVids.Data;

/// <inheritdoc cref="IMongoDbConnectionFactory" />
public sealed class MongoDbConnectionFactory : IMongoDbConnectionFactory
{
    public MongoDbConnectionFactory(ILogger<MongoDbConnectionFactory> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <inheritdoc />
    public MongoClientBase Create()
    {
        _logger.LogInformation("Trying to create connection to MongoDB.");
        return TryToConnectToMongoDb();
    }

    private MongoClient TryToConnectToMongoDb()
    {
        var connectionString = TryToGetMongoDbConnectionString();

        try
        {
            return new MongoClient(connectionString);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to connect to MongoDB using configurated connection string.");
            throw;
        }
    }

    private string TryToGetMongoDbConnectionString()
    {
        const string RAW_VIDEO_DB_CONNECTION_STRING_NAME = "raw-video-db";
        var connectionString = _configuration.GetConnectionString(RAW_VIDEO_DB_CONNECTION_STRING_NAME);
        if (connectionString is null)
        {
            var ex = new InvalidOperationException($"Unable to find connection string '{RAW_VIDEO_DB_CONNECTION_STRING_NAME}' for MongoDB.");
            _logger.LogCritical(ex, "Unable to find connection string '{ConnectionStringName}' for MongoDB.", RAW_VIDEO_DB_CONNECTION_STRING_NAME);
            throw ex;
        }

        return connectionString;
    }

    private readonly ILogger<MongoDbConnectionFactory> _logger;
    private readonly IConfiguration _configuration;
}
