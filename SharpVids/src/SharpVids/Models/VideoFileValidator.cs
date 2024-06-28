using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using SharpVids.Options;

namespace SharpVids.Models;

public class VideoFileValidator : AbstractValidator<IBrowserFile>
{
    public VideoFileValidator(IOptionsMonitor<UploadOptions> uploadOptions)
    {
        const int MB = 1024 * 1024;
        var fileSizeLimitInBytes = uploadOptions.CurrentValue.FileSizeLimitInMB * MB;

        RuleFor(file => file.Size)
            .Must(size =>
            {
                return size < fileSizeLimitInBytes;
            })
            .WithMessage($"Uploaded videos must be smaller than {uploadOptions.CurrentValue.FileSizeLimitInMB}MB.");

        RuleFor(file => file.ContentType)
            .Must(IsVideoMimeType)
            .WithMessage("Only video files can be uploaded.");
    }

    private static bool IsVideoMimeType(string mimeType)
    {
        return mimeType.StartsWith("video/");
    }
}
