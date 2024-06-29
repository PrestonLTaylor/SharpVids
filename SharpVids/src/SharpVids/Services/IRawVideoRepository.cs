using Microsoft.AspNetCore.Components.Forms;
using MongoDB.Bson;

namespace SharpVids.Services;

/// <summary>
/// Handles persistance for raw videos uploaded by a user.
/// </summary>
public interface IRawVideoRepository
{
    /// <summary>
    /// Uploads a raw video to our raw video database.
    /// </summary>
    /// <param name="videoFile">The <see cref="IBrowserFile"/> a user is trying to upload.</param>
    /// <param name="updateCallback">A callback that is called when the number of bytes uploaded has changed.</param>
    /// <returns>An <see cref="ObjectId"/> used to reference the video file inside of our database.</returns>
    public Task<ObjectId> UploadRawVideoAsync(IBrowserFile videoFile, Action<long> uploadCallback);

    /// <summary>
    /// Creates a video metadata document inside of our video metadata collection.
    /// </summary>
    /// <param name="videoId">The id of the raw video inside of GridFS.</param>
    public Task AddVideoMetadataAsync(ObjectId videoId);
}
