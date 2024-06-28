using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;

namespace SharpVids.Services;

/// <inheritdoc cref="IUploadService"/>
public sealed class UploadService : IUploadService
{
    public UploadService(IValidator<IBrowserFile> fileValidator, IRawVideoDbService dbService)
    {
        _fileValidator = fileValidator;
        _dbService = dbService;
    }

    /// <inheritdoc/>
    public void OnFileChanged(InputFileChangeEventArgs args)
    {
        ResetUploadState();
        _fileToUpload = args.File;
    }

    /// <inheritdoc/>
    public async Task TryToUploadVideoAsync(Action<long> uploadCallback)
    {
        ResetUploadState();

        if (_fileToUpload is null) return;

        var validationResult = _fileValidator.Validate(_fileToUpload);
        if (!validationResult.IsValid)
        {
            _uploadErrors = validationResult
                .Errors
                .Select(x => x.ErrorMessage)
                .ToList();

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

    private async Task TryToUploadRawVideoFromUserAsync(Action<long> uploadCallback)
    {
        // FIXME: Allow a user to cancel an upload using cancellation tokens
        // FIXME: Implement a replica set for our mongodb database so we can use transactions
        var videoId = await _dbService.UploadRawVideoAsync(_fileToUpload!, uploadCallback);

        await _dbService.AddVideoMetadataAsync(videoId);
    }

    public long VideoFileSize { get => _fileToUpload?.Size ?? 0; }

    private List<string> _uploadErrors = [];
    public IReadOnlyList<string> UploadErrors { get => _uploadErrors; }

    private readonly IValidator<IBrowserFile> _fileValidator;
    private readonly IRawVideoDbService _dbService;
    private IBrowserFile? _fileToUpload;
}
