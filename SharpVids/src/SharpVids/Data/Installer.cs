namespace SharpVids.Data;

public static class Installer
{
    public static IServiceCollection AddRawVideoDbServices(this IServiceCollection services)
    {
        services.AddTransient<IMongoDbConnectionFactory, MongoDbConnectionFactory>();

        services.AddTransient<IRawVideoDb, RawVideoDb>();

        return services;
    }
}
