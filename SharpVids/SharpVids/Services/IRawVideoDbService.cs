using Microsoft.AspNetCore.Components.Forms;
using MongoDB.Bson;

namespace SharpVids.Services;

public interface IRawVideoDbService
{
    public Task<ObjectId> UploadRawVideoAsync(IBrowserFile videoFile, Action<long> uploadCallback);
}
