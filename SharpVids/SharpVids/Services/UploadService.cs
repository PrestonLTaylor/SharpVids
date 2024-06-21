using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using SharpVids.Options;

namespace SharpVids.Services;

public sealed class UploadService : IUploadService
{
    public UploadService(IOptionsMonitor<UploadOptions> uploadOptions, IRawVideoDbService dbService)
    {
        _uploadOptions = uploadOptions;
        _dbService = dbService;
    }

    public void OnFileChanged(InputFileChangeEventArgs args)
    {
        ResetUploadState();
        _fileToUpload = args.File;
    }

    public async Task TryToUploadVideoAsync(Action<long> uploadCallback)
    {
        ResetUploadState();

        if (_fileToUpload is null) return;

        var fileSizeLimitInBytes = _uploadOptions.CurrentValue.FileSizeLimitInMB * MB;
        if (_fileToUpload.Size > fileSizeLimitInBytes)
        {
            var videoFileInMb = _fileToUpload.Size / (float)MB;
            _uploadErrors.Add($"Attempted to upload a video of size {videoFileInMb:N2}MB however, only videos smaller than {_uploadOptions.CurrentValue.FileSizeLimitInMB}MB are allowed.");
            return;
        }

        if (!IsVideoMimeType(_fileToUpload.ContentType))
        {
            _uploadErrors.Add($"Attempted to upload a non-video file, only video files are allowed.");
            return;
        }

        try
        {
            await TryToUploadRawVideoFromUserAsync(uploadCallback);
        }
        catch (Exception ex)
        {
            // FIXME: Catch specific exceptions and report human readable error messages
            _uploadErrors.Add(ex.Message);
        }
    }

    private void ResetUploadState()
    {
        _uploadErrors.Clear();
    }

    private static bool IsVideoMimeType(string mimeType)
    {
        return mimeType.StartsWith("video/");
    }

    private async Task TryToUploadRawVideoFromUserAsync(Action<long> uploadCallback)
    {
        // FIXME: Allow a user to cancel an upload using cancellation tokens
        // FIXME: Implement a replica set for our mongodb database so we can use transactions
        var videoId = await _dbService.UploadRawVideoAsync(_fileToUpload!, uploadCallback);

        await _dbService.AddVideoMetadataAsync(videoId);
    }

    public long VideoFileSize { get => _fileToUpload?.Size ?? 0; }

    private readonly List<string> _uploadErrors = [];
    public IReadOnlyList<string> UploadErrors { get => _uploadErrors; }

    private const int MB = 1024 * 1024;

    private readonly IOptionsMonitor<UploadOptions> _uploadOptions;
    private readonly IRawVideoDbService _dbService;
    private IBrowserFile? _fileToUpload;
}
