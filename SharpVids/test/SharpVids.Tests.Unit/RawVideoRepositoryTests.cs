using FluentAssertions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using NSubstitute;
using SharpVids.Data;
using SharpVids.Models;
using SharpVids.Options;
using SharpVids.Services;

namespace SharpVids.Tests.Unit;

public sealed class RawVideoRepositoryTests
{
    public RawVideoRepositoryTests()
    {
        _sut = new(_db, NullLogger<RawVideoRepository>.Instance, _options);

        // Setup default behaviour for our tests
        _db.GetBucketFromDb(EXPECTED_DB_NAME)
            .Returns(_videoBucket);

        _db.GetCollection<RawVideoMetadataModel>(EXPECTED_DB_NAME, EXPECTED_METADATA_COLLECTION_NAME)
            .Returns(_metadataCollection);

        _videoBucket.OpenUploadStreamAsync(Arg.Any<ObjectId>(), DEFAULT_VIDEO_FILE_NAME)
            .Returns(_uploadStream);

        _options.CurrentValue
            .Returns(new UploadOptions() { FileSizeLimitInMB = DEFAULT_FILE_SIZE_LIMIT_IN_MB });

        _uploadFile.Name
            .Returns(DEFAULT_VIDEO_FILE_NAME);

        _uploadFile.Size
            .Returns(DEFAULT_VIDEO_FILE_SIZE);

        _uploadFile.OpenReadStream(DEFAULT_FILE_SIZE_LIMIT_IN_MB * 1024 * 1024)
            .Returns(_readStream);

        _readStream.ReadAsync(Arg.Any<Memory<byte>>())
            .Returns(info => ValueTask.FromResult(info.Arg<Memory<byte>>().Length))
            .AndDoes(info => _readStream.Position += info.Arg<Memory<byte>>().Length);

        _uploadStream.WriteAsync(Arg.Any<ReadOnlyMemory<byte>>())
            .Returns(ValueTask.CompletedTask)
            .AndDoes(info => _uploadStream.Position += info.Arg<ReadOnlyMemory<byte>>().Length);
    }

    [Fact]
    public async Task UploadRawVideoAsync_ShouldReadTheWholeVideoFile_WhenProvidedValidBrowserFile()
    {
        // Arrange

        // Act
        await _sut.UploadRawVideoAsync(_uploadFile, _updateCallback);

        // Assert
        _readStream.Position
            .Should().Be(DEFAULT_VIDEO_FILE_SIZE);

        await _readStream.Received(EXPECTED_STREAM_CALLS)
            .ReadAsync(Arg.Any<Memory<byte>>());
    }

    [Fact]
    public async Task UploadRawVideoAsync_ShouldUploadTheWholeVideo_WhenProvidedValidBrowserFile()
    {
        // Arrange

        // Act
        await _sut.UploadRawVideoAsync(_uploadFile, _updateCallback);

        // Assert
        _uploadStream.Position
            .Should().Be(DEFAULT_VIDEO_FILE_SIZE);

        await _uploadStream.Received(EXPECTED_STREAM_CALLS)
            .WriteAsync(Arg.Any<ReadOnlyMemory<byte>>());
    }

    [Fact]
    public async Task UploadRawVideoAsync_ShouldCallUploadCallback_WithExpectedUploadedBytes_WhenProvidingValidBrowserFile()
    {
        // Arrange
        // Ensures that the update callback is called with the actual number of uploaded bytes (The read stream's position).
        _updateCallback
            .When(callback => callback.Invoke(Arg.Any<long>()))
            .Do(info =>
            {
                info.Arg<long>().Should().Be(_readStream.Position);
            });

        // Act
        await _sut.UploadRawVideoAsync(_uploadFile, _updateCallback);

        // Assert
        _updateCallback.Received(EXPECTED_STREAM_CALLS)
            .Invoke(Arg.Any<long>());
    }

    [Fact]
    public async Task AddVideoMetadataAsync_ShouldInsertOneMetadata_WithExpectedObjectId_WhenProvidingSameObjectId()
    {
        // Arrange
        var expectedObjectId = ObjectId.GenerateNewId();

        // Ensures correct video id is inserted
        _metadataCollection
            .InsertOneAsync(Arg.Any<RawVideoMetadataModel>())
            .Returns(Task.CompletedTask)
            .AndDoes(info =>
            {
                info.Arg<RawVideoMetadataModel>().VideoId.Should().Be(expectedObjectId);
            });

        // Act
        await _sut.AddVideoMetadataAsync(expectedObjectId);

        // Assert
        await _metadataCollection
            .Received(1)
            .InsertOneAsync(Arg.Any<RawVideoMetadataModel>());
    }

    const string EXPECTED_DB_NAME = "raw-videos";
    const string EXPECTED_METADATA_COLLECTION_NAME = "raw-video-metadata";
    const int EXPECTED_STREAM_CALLS = (int)(DEFAULT_VIDEO_FILE_SIZE / DEFAULT_CHUNK_SIZE);
    const string DEFAULT_VIDEO_FILE_NAME = "test.mp4";
    const long DEFAULT_VIDEO_FILE_SIZE = 5 * 1024 * 1024;
    const long DEFAULT_CHUNK_SIZE = 80 * 1024;
    const int DEFAULT_FILE_SIZE_LIMIT_IN_MB = 10;

    private readonly RawVideoRepository _sut;

    private readonly IRawVideoDb _db = Substitute.For<IRawVideoDb>();
    private readonly IGridFSBucket _videoBucket = Substitute.For<IGridFSBucket>();
    private readonly GridFSUploadStream<ObjectId> _uploadStream = Substitute.For<GridFSUploadStream<ObjectId>>();
    private readonly IMongoCollection<RawVideoMetadataModel> _metadataCollection = Substitute.For<IMongoCollection<RawVideoMetadataModel>>();

    private readonly IOptionsMonitor<UploadOptions> _options = Substitute.For<IOptionsMonitor<UploadOptions>>();

    private readonly IBrowserFile _uploadFile = Substitute.For<IBrowserFile>();
    private readonly Stream _readStream = Substitute.For<Stream>();

    private readonly Action<long> _updateCallback = Substitute.For<Action<long>>();
}
