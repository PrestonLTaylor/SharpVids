using HttpMultipartParser;

namespace SharpVids.Utilities;

public sealed class StreamingMultipartFormDataParserFactory : IStreamingMultipartFormDataParserFactory
{
    public IStreamingMultipartFormDataParser Create(Stream stream) => new StreamingMultipartFormDataParser(stream);
}
