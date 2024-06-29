using FluentValidation;

namespace SharpVids.Services;

public static class Installer
{
    public static IServiceCollection AddSharpVidsServices(this IServiceCollection services)
    {
        services.AddTransient<IRawVideoRepository, RawVideoRepository>();

        services.AddTransient<IUploadService, UploadService>();

        services.AddValidatorsFromAssembly(typeof(Installer).Assembly);

        return services;
    }
}
