using Microsoft.AspNetCore.Components.Forms;

namespace SharpVids.Services;

public interface IUploadService
{
    public void OnFileChanged(InputFileChangeEventArgs args);

    public Task TryToUploadVideoAsync(Action<long> uploadCallback);

    public long VideoFileSize { get; }
    public IReadOnlyList<string> UploadErrors { get; }
}
