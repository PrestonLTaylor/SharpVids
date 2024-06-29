using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components.Forms;
using MongoDB.Bson;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SharpVids.Services;

namespace SharpVids.Tests.Unit;

public sealed class UploadServiceTests
{
    public UploadServiceTests()
    {
        _sut = new(_validator, _dbService);
    }

    [Fact]
    public async Task TryToUploadVideoAsync_ShouldDoNothing_WhenFileToUploadIsNull()
    {
        // Arrange

        // Act
        await _sut.TryToUploadVideoAsync(_ => { });

        // Assert
        _validator.ReceivedCalls()
            .Should().BeEmpty();

        _dbService.ReceivedCalls()
            .Should().BeEmpty();

        _sut.UploadErrors
            .Should().BeEmpty();
    }

    [Fact]
    public async Task TryToUploadVideoAsync_ShouldNotUploadVideo_WhenValidationFails()
    {
        // Arrange
        SetValidationResult([new ValidationFailure()]);

        SetUploadFile();

        // Act
        await _sut.TryToUploadVideoAsync(_ => { });

        // Assert
        AssertValidationWasCalled();

        _dbService.ReceivedCalls()
            .Should().BeEmpty();
    }

    [Fact]
    public async Task TryToUploadVideoAsync_ShouldSetErrorMessages_WhenValidationFails()
    {
        // Arrange
        const string EXPECTED_ERROR_MESSAGE = "test";
        SetValidationResult([new ValidationFailure("", EXPECTED_ERROR_MESSAGE)]);

        SetUploadFile();

        // Act
        await _sut.TryToUploadVideoAsync(_ => { });
        var errors = _sut.UploadErrors;

        // Assert
        errors.Should().HaveCount(1);

        errors[0].Should().Be(EXPECTED_ERROR_MESSAGE);
    }

    [Fact]
    public async Task TryToUploadVideoAsync_ShouldSetErrorMessages_WhenUploadingFails()
    {
        // Arrange
        const string EXPECTED_ERROR_MESSAGE = "test";

        SetValidationResult([]);

        SetUploadFile();

        _dbService.UploadRawVideoAsync(_uploadFile, Arg.Any<Action<long>>())
            .ThrowsAsync(new Exception(EXPECTED_ERROR_MESSAGE));

        // Act
        await _sut.TryToUploadVideoAsync(_ => { });
        var errors = _sut.UploadErrors;

        // Assert
        errors.Should().HaveCount(1);

        errors[0].Should().Be(EXPECTED_ERROR_MESSAGE);
    }

    [Fact]
    public async Task TryToUploadVideoAsync_ShouldSetErrorMessages_WhenAddingMetadataFails()
    {
        // Arrange
        const string EXPECTED_ERROR_MESSAGE = "test";

        SetValidationResult([]);

        SetUploadFile();

        _dbService.AddVideoMetadataAsync(Arg.Any<ObjectId>())
            .ThrowsAsync(new Exception(EXPECTED_ERROR_MESSAGE));

        // Act
        await _sut.TryToUploadVideoAsync(_ => { });
        var errors = _sut.UploadErrors;

        // Assert
        errors.Should().HaveCount(1);

        errors[0].Should().Be(EXPECTED_ERROR_MESSAGE);
    }

    [Fact]
    public async Task TryToUploadVideoAsync_ShouldHaveNoErrors_WhenUploadingIsSuccessful()
    {
        // Arrange
        SetValidationResult([]);

        SetUploadFile();

        // Act
        await _sut.TryToUploadVideoAsync(_ => { });
        var errors = _sut.UploadErrors;

        // Assert
        errors.Should().BeEmpty(); 
    }

    [Fact]
    public async Task TryToUploadVideoAsync_ShouldUseProvidedFileAndUploadCallback_WhenTryingToUpload()
    {
        // Arrange
        Action<long> action = _ => { };

        SetValidationResult([]);

        SetUploadFile();

        // Act
        await _sut.TryToUploadVideoAsync(action);

        // Assert
        await _dbService.Received(1)
            .UploadRawVideoAsync(_uploadFile, action);
    }

    [Fact]
    public async Task TryToUploadVideoAsync_ShouldUseObjectIdOfUploadedFile_WhenAddingMetadata()
    {
        // Arrange
        var expectedObjectId = ObjectId.GenerateNewId();
        _dbService.UploadRawVideoAsync(_uploadFile, Arg.Any<Action<long>>())
            .Returns(expectedObjectId);

        SetValidationResult([]);

        SetUploadFile();

        // Act
        await _sut.TryToUploadVideoAsync(_ => { });

        // Assert
        await _dbService.Received(1)
            .AddVideoMetadataAsync(expectedObjectId);
    }

        [Fact]
    public void VideoFileSize_ShouldBeZero_WhenFileToUploadIsNull()
    {
        // Arrange

        // Act
        var fileSize = _sut.VideoFileSize;

        // Assert
        fileSize.Should().Be(0);
    }

    [Fact]
    public void VideoFileSize_ShouldBeUploadFileSize_WhenFileToUploadIsNotNull()
    {
        // Arrange
        const long EXPECTED_FILE_SIZE = 10;
        _uploadFile.Size
            .Returns(EXPECTED_FILE_SIZE);

        SetUploadFile();

        // Act
        var fileSize = _sut.VideoFileSize;

        // Assert
        fileSize.Should().Be(EXPECTED_FILE_SIZE);
    }

    private void SetUploadFile()
    {
        _sut.OnFileChanged(new([_uploadFile]));
    }

    private void SetValidationResult(IEnumerable<ValidationFailure> validationFailures)
    {
        var validationResult = new ValidationResult(validationFailures);
        _validator.Validate(_uploadFile)
            .Returns(validationResult);
    }

    private void AssertValidationWasCalled()
    {
        _validator.Received(1).Validate(_uploadFile);
    }

    private readonly UploadService _sut;

    private readonly IValidator<IBrowserFile> _validator = Substitute.For<IValidator<IBrowserFile>>();

    private readonly IRawVideoDbService _dbService = Substitute.For<IRawVideoDbService>();

    private readonly IBrowserFile _uploadFile = Substitute.For<IBrowserFile>();
}
