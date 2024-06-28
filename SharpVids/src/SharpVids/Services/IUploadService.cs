using Microsoft.AspNetCore.Components.Forms;

namespace SharpVids.Services;

/// <summary>
/// The service used by the upload page to handling uploading of video files to our raw video store.
/// </summary>
public interface IUploadService
{
    /// <summary>
    /// This signifies to the upload service that a different file was selected by the user and should be updated internally.
    /// </summary>
    /// <param name="args">The args that triggered this event.</param>
    public void OnFileChanged(InputFileChangeEventArgs args);

    /// <summary>
    /// Validates and start to upload the file that was supplied to the user.
    /// This function will communicate with the caller how many bytes were uploaded so far.
    /// </summary>
    /// <param name="uploadCallback">A callback that is called when the number of bytes uploaded has changed.</param>
    public Task TryToUploadVideoAsync(Action<long> uploadCallback);

    public long VideoFileSize { get; }
    public IReadOnlyList<string> UploadErrors { get; }
}
