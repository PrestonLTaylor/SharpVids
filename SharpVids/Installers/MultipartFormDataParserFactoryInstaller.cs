using SharpVids.Utilities;

namespace SharpVids.Installers;

static public class MultipartFormDataParserFactoryInstaller
{
    static public IServiceCollection AddMultipartFormDataParserFactory(this IServiceCollection services)
    {
        services.AddTransient<IStreamingMultipartFormDataParserFactory, StreamingMultipartFormDataParserFactory>();
        return services;
    }
}
