using HttpMultipartParser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharpVids.Controllers;
using SharpVids.Data.Generators;
using SharpVids.Data.Repositories;
using SharpVids.Models;
using SharpVids.Utilities;
using System.Security.Claims;

namespace SharpVids.UnitTest;

internal sealed class VideoControllerTests
{
    [Test]
    public async Task Video_ReturnsNotFound_WhenSuppliedInvalidId()
    {
        // Arrange
        _videoRepositoryMock.Setup(m => m.GetVideoByIdAsync(It.IsAny<Guid>())).ReturnsAsync((VideoModel?)null);

		// Act
		var videoController = new VideoController(_userRepositoryMock.Object, _videoRepositoryMock.Object, _multipartFormDataParserFactoryMock.Object);
		var result = await videoController.Video(Guid.NewGuid()) as NotFoundResult;

		// Assert
		Assert.That(result, Is.Not.Null);
	}

	[Test]
	public async Task Video_ReturnsSpecifiedVideo_WhenProvidedId()
	{
        // Arrange
        var fakeVideos = GenerateFakeVideos();
        var specificVideo = fakeVideos.First();
		_videoRepositoryMock.Setup(m => m.GetVideoByIdAsync(specificVideo.Id)).ReturnsAsync(specificVideo);

		// Act
		var videoController = new VideoController(_userRepositoryMock.Object, _videoRepositoryMock.Object, _multipartFormDataParserFactoryMock.Object);
		var viewResult = await videoController.Video(specificVideo.Id) as ViewResult;
        var result = viewResult?.Model as VideoModel;

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Id, Is.EqualTo(specificVideo.Id));
	}

	[Test]
    public void Upload_AlwaysReturnsViewResult()
    {
        // Arrange

        // Act
        var videoController = new VideoController(_userRepositoryMock.Object, _videoRepositoryMock.Object, _multipartFormDataParserFactoryMock.Object);
        var result = videoController.Upload() as ViewResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task SubmitUpload_ReturnsBadRequest_WhenNoTitleIsSupplied()
    {
        // Arrange
        SetupMultiformDataParserWith(null, _fakeDescription, _fakeThumbnailUrl, _fakeVideo);
        var httpContextMock = CreateHttpContextMockWithRequest();

        // Act
        var videoController = new VideoController(_userRepositoryMock.Object, _videoRepositoryMock.Object, _multipartFormDataParserFactoryMock.Object);
        videoController.ControllerContext.HttpContext = httpContextMock.Object;
        var result = await videoController.SubmitUpload() as BadRequestObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task SubmitUpload_ReturnsBadRequest_WhenNoDescriptionIsSupplied()
    {
        // Arrange
        SetupMultiformDataParserWith(_fakeTitle, null, _fakeThumbnailUrl, _fakeVideo);
        var httpContextMock = CreateHttpContextMockWithRequest();

        // Act
        var videoController = new VideoController(_userRepositoryMock.Object, _videoRepositoryMock.Object, _multipartFormDataParserFactoryMock.Object);
        videoController.ControllerContext.HttpContext = httpContextMock.Object;
        var result = await videoController.SubmitUpload() as BadRequestObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task SubmitUpload_ReturnsBadRequest_WhenNoThumbnailUrlIsSupplied()
    {
        // Arrange
        SetupMultiformDataParserWith(_fakeTitle, _fakeDescription, null, _fakeVideo);
        var httpContextMock = CreateHttpContextMockWithRequest();

        // Act
        var videoController = new VideoController(_userRepositoryMock.Object, _videoRepositoryMock.Object, _multipartFormDataParserFactoryMock.Object);
        videoController.ControllerContext.HttpContext = httpContextMock.Object;
        var result = await videoController.SubmitUpload() as BadRequestObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task SubmitUpload_ReturnsBadRequest_WhenNoVideoSupplied()
    {
        // Arrange
        SetupMultiformDataParserWith(_fakeTitle, _fakeDescription, _fakeThumbnailUrl, null);
        var httpContextMock = CreateHttpContextMockWithRequest();

        // Act
        var videoController = new VideoController(_userRepositoryMock.Object, _videoRepositoryMock.Object, _multipartFormDataParserFactoryMock.Object);
        videoController.ControllerContext.HttpContext = httpContextMock.Object;
        var result = await videoController.SubmitUpload() as BadRequestObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task SubmitUpload_AddsVideoToRepository_WhenSuppliedValidVideo()
    {
        // Arrange
        SetupMultiformDataParserWith(_fakeTitle, _fakeDescription, _fakeThumbnailUrl, _fakeVideo);
        var httpContextMock = CreateHttpContextMockWithRequest();
        var fakeUser = GenerateFakeUser();
        _userRepositoryMock.Setup(m => m.GetUserByClaimAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(fakeUser);
        _videoRepositoryMock.Setup(m => m.AddVideoAsync(It.IsAny<VideoModel>())).Verifiable();

        // Act
        var videoController = new VideoController(_userRepositoryMock.Object, _videoRepositoryMock.Object, _multipartFormDataParserFactoryMock.Object);
        videoController.ControllerContext.HttpContext = httpContextMock.Object;
        var result = await videoController.SubmitUpload() as CreatedAtActionResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            _videoRepositoryMock.VerifyAll();
        });
    }

    private void SetupMultiformDataParserWith(string? title, string? description, string? thumbnailUrl, byte[]? video)
    {
        var multipartFormDataParserMock = CreateMultipartFormDataParserMock();
        multipartFormDataParserMock.SetupAllProperties();
        multipartFormDataParserMock.Setup(m => m.RunAsync(It.IsAny<CancellationToken>())).Callback(() =>
        {
            multipartFormDataParserMock.Object.ParameterHandler(new ParameterPart("Title", title));
            multipartFormDataParserMock.Object.ParameterHandler(new ParameterPart("Description", description));
            multipartFormDataParserMock.Object.ParameterHandler(new ParameterPart("ThumbnailUrl", thumbnailUrl));
            if (video is not null)
                multipartFormDataParserMock.Object.FileHandler("", "", "", "", video, video.Length, 0, null);
        });
    }

    private Mock<IStreamingMultipartFormDataParser> CreateMultipartFormDataParserMock()
    {
        var mock = new Mock<IStreamingMultipartFormDataParser>();
        _multipartFormDataParserFactoryMock.Setup(m => m.Create(It.IsAny<Stream>())).Returns(mock.Object);
        return mock;
    }

    private Mock<HttpContext> CreateHttpContextMockWithRequest()
    {
        var httpContextMock = new Mock<HttpContext>();
        var httpRequestMock = new Mock<HttpRequest>();
        httpContextMock.Setup(m => m.Request).Returns(httpRequestMock.Object);
        return httpContextMock;
    }

    private List<VideoModel> GenerateFakeVideos()
    {
		FakeVideoGenerator fakeVideoGenerator = new();
		int numberOfVideos = Random.Shared.Next(1, 100);
		return fakeVideoGenerator.GenerateForever().Take(numberOfVideos).ToList();
	}

    private UserModel GenerateFakeUser()
    {
        var fakeUserGenerator = new FakeUserGenerator();
        return fakeUserGenerator.GenerateForever().First();
    }

    private readonly Mock<IUserRepository> _userRepositoryMock = new();
	private readonly Mock<IVideoRepository> _videoRepositoryMock = new();
	private readonly Mock<IStreamingMultipartFormDataParserFactory> _multipartFormDataParserFactoryMock = new();

    private const string _fakeTitle = "FakeTitle";
    private const string _fakeDescription = "FakeDescription";
    private const string _fakeThumbnailUrl = "https://www.fake.url/thumbnail.png";
    // TODO: Create fake signatures for video file formats when we do file format validation
    private readonly byte[] _fakeVideo = { 0, 0, 0, 0 };
}
