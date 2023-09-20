using HttpMultipartParser;

namespace SharpVids.Utilities;

public interface IStreamingMultipartFormDataParserFactory
{
    public IStreamingMultipartFormDataParser Create(Stream stream);
}
