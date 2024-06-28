namespace SharpVids.Options;

public sealed class UploadOptions
{
    public const string Section = "UploadOptions";

    /// <summary>
    /// Specifies the limit of the video files that can be uploaded to the server
    /// </summary>
    public int FileSizeLimitInMB { get; set; } = DefaultFileSizeLimitInMB;

    private const int DefaultFileSizeLimitInMB = 50;
}
