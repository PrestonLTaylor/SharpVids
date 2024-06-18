using Microsoft.AspNetCore.Components.Forms;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using SharpVids.Data;
using Microsoft.Extensions.Options;
using SharpVids.Options;
using SharpVids.Models;

namespace SharpVids.Services;

/// <summary>
/// Handles operations between the front-end and the MongoDB service (such as uploading raw videos and creating metadata).
/// </summary>
public sealed class RawVideoDbService : IRawVideoDbService
{
    public RawVideoDbService(RawVideoDb dbClient, ILogger<RawVideoDbService> logger, IOptionsMonitor<UploadOptions> uploadOptions)
    {
        _dbClient = dbClient;
        _logger = logger;
        _uploadOptions = uploadOptions;
    }

    /// <inheritdoc/>
    public async Task<ObjectId> UploadRawVideoAsync(IBrowserFile videoFile, Action<long> updateCallback)
    {
        var videoId = ObjectId.GenerateNewId();
        _logger.LogInformation("Uploading a raw video with the id {VideoId} and name {VideoName}", videoId, videoFile.Name);

        var rawVideosBucket = GetRawVideosBucket();

        using var userVideoStream = videoFile.OpenReadStream(FileSizeLimitInBytes);
        using var dbVideoStream = await rawVideosBucket.OpenUploadStreamAsync(videoId, videoFile.Name);

        var uploadChunk = new byte[UPLOAD_BUFFER_SIZE];
        long numberOfBytesUploaded = 0;
        while (numberOfBytesUploaded != videoFile.Size)
        {
            // NOTE: We don't use CopyToAsync as that won't allow for updates of the number of uploaded bytes
            await userVideoStream.ReadAsync(uploadChunk);
            await dbVideoStream.WriteAsync(uploadChunk);
            numberOfBytesUploaded = userVideoStream.Position;
            updateCallback(numberOfBytesUploaded);
        }

        _logger.LogInformation("Uploaded a raw video with the id {VideoId} and name {VideoName}", videoId, videoFile.Name);
        return videoId;
    }

    /// <inheritdoc/>
    public async Task AddVideoMetadataAsync(ObjectId videoId)
    {
        _logger.LogInformation("Creating a video metadata document with the id {VideoId}", videoId);

        var collection = GetRawVideoMetadataCollection();

        var metadata = new RawVideoMetadataModel { VideoId = videoId };
        await collection.InsertOneAsync(metadata);
    }

    private IMongoCollection<RawVideoMetadataModel> GetRawVideoMetadataCollection()
    {
        var db = GetRawVideoDatabase();
        return db.GetCollection<RawVideoMetadataModel>(RAW_VIDEO_METADATA_COLLECTION_NAME);
    }

    private GridFSBucket GetRawVideosBucket()
    {
        var db = GetRawVideoDatabase();
        return new GridFSBucket(db);
    }

    private IMongoDatabase GetRawVideoDatabase()
    {
        return _dbClient.Client.GetDatabase(RAW_VIDEO_DB_NAME);
    }

    const string RAW_VIDEO_DB_NAME = "raw-videos";
    const string RAW_VIDEO_METADATA_COLLECTION_NAME = "raw-video-metadata";
    const int KB = 1024;
    const int MB = 1024 * 1024;
    const int UPLOAD_BUFFER_SIZE = 80 * KB;

    private int FileSizeLimitInBytes => _uploadOptions.CurrentValue.FileSizeLimitInMB * MB;

    private readonly RawVideoDb _dbClient;
    private readonly ILogger<RawVideoDbService> _logger;
    private readonly IOptionsMonitor<UploadOptions> _uploadOptions;
}
