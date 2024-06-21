﻿namespace SharpVids.Services;

public static class Installer
{
    public static IServiceCollection AddSharpVidsServices(this IServiceCollection services)
    {
        services.AddTransient<IRawVideoDbService, RawVideoDbService>();

        services.AddTransient<IUploadService, UploadService>();

        return services;
    }
}