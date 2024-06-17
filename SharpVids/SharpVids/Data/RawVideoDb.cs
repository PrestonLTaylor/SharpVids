using MongoDB.Driver;

namespace SharpVids.Data;

/// <summary>
/// A wrapper for connecting to our MongoDB service to be used as a raw video database.
/// </summary>
public sealed class RawVideoDb
{
    public RawVideoDb(ILogger<RawVideoDb> logger, IConfiguration configuration)
    {
        _logger = logger;
        Client = TryToConnectToRawVideoDb(configuration);
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
        var connectionString = configuration.GetConnectionString(RAW_VIDEO_DB_CONNECTION_STRING_NAME);
        if (connectionString is null)
        {
            var ex = new InvalidOperationException($"Unable to find connection string '{RAW_VIDEO_DB_CONNECTION_STRING_NAME}' for raw video db.");
            _logger.LogCritical(ex, "Failed to connect to raw video db.");
            throw ex;
        }

        return connectionString;
    }

    public MongoClient Client { get; }

    const string RAW_VIDEO_DB_CONNECTION_STRING_NAME = "raw-video-db";

    private readonly ILogger<RawVideoDb> _logger;
}

public static class RawVideoDbInstaller
{
    public static IServiceCollection AddRawVideoDb(this IServiceCollection services)
    {
        services.AddTransient<RawVideoDb>();

        return services;
    }
}
