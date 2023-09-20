using SharpVids.Models;

namespace SharpVids.Data.Repositories;

public interface IVideoRepository
{
    public Task<List<VideoModel>> GetVideosAsync();
    public Task<VideoModel?> GetVideoByIdAsync(Guid id);

    public Task AddVideoAsync(VideoModel video);
}
