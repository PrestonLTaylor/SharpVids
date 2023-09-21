using HttpMultipartParser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpVids.Data.Repositories;
using SharpVids.Filters;
using SharpVids.Models;
using SharpVids.Utilities;

namespace SharpVids.Controllers;

public sealed class VideoController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IVideoRepository _videoRepository;
    private readonly IStreamingMultipartFormDataParserFactory _multipartFormDataParserFactory;

    public VideoController(IUserRepository userRepository, IVideoRepository videoRepository, IStreamingMultipartFormDataParserFactory multipartFormDataParserFactory)
    {
        _userRepository = userRepository;
        _videoRepository = videoRepository;
        _multipartFormDataParserFactory = multipartFormDataParserFactory;
    }

	// TODO: Retrieving videos causes the whole video file to be loaded, making this slow, optimize this later!
	public async Task<IActionResult> Video([FromRoute]Guid id)
    {
        var video = await _videoRepository.GetVideoByIdAsync(id);
        if (video is null)
        {
            return NotFound();
        }

        return View(video);
    }

    public async Task<IActionResult> Videos()
    {
        var videos = await _videoRepository.GetVideosAsync();
        return View(videos);
    }

    [Authorize]
    [GenerateAntiforgeryTokenCookie]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost($"{{controller}}/{nameof(Upload)}")]
    [Authorize]
    [DisableFormValueModelBinding]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitUpload()
    {
        var parser = _multipartFormDataParserFactory.Create(Request.Body);
        var parameterList = CreateReceivingParameterNameToDataDictionary(parser);
        var fileData = CreateReceivingFileDataList(parser);
        await parser.RunAsync();

        var title = parameterList.GetValueOrDefault("Title");
        var description = parameterList.GetValueOrDefault("Description");
        var thumbnailUrl = parameterList.GetValueOrDefault("ThumbnailUrl");
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(thumbnailUrl))
        {
            // TODO: Better error messages
            ModelState.AddModelError(string.Empty, "The title, description or thumbnail url is missing.");
            return BadRequest(ModelState);
        }

        // TODO: File signature checking for allowed video types
        if (fileData.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "The uploaded video was corrupted.");
            return BadRequest(ModelState);
        }

        var videoModel = await CreateVideoForCurrentUserAsync(title, description, thumbnailUrl, fileData);
        await _videoRepository.AddVideoAsync(videoModel);

        return CreatedAtAction(nameof(Video), videoModel.Id, null);
    }

    private Dictionary<string, string> CreateReceivingParameterNameToDataDictionary(IStreamingMultipartFormDataParser parser)
    {
        Dictionary<string, string> map = new();
        parser.ParameterHandler += parameter => map.Add(parameter.Name, parameter.Data);
        return map;
    }

    private List<byte> CreateReceivingFileDataList(IStreamingMultipartFormDataParser parser)
    {
        List<byte> fileData = new();

        parser.FileHandler += (name, fileName, type, disposition, buffer, bytes, partNumber, additionalProperites) =>
        {
            fileData.AddRange(buffer);
        };

        return fileData;
    }

    private async Task<VideoModel> CreateVideoForCurrentUserAsync(string title, string description, string thumbnailUrl, List<byte> fileData)
    {
        var creator = await _userRepository.GetUserByClaimAsync(User);

        return new VideoModel
        {
            Id = Guid.NewGuid(),
            CreatorId = creator!.Id,
            Title = title,
            Description = description,
            UploadDate = DateTimeOffset.Now,
            ThumbnailUrl = thumbnailUrl,
            VideoBytes = fileData,
        };
    }
}
