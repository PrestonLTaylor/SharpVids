using Microsoft.EntityFrameworkCore;
using SharpVids.Models;

namespace SharpVids.Data.Repositories;

public sealed class VideoRepository : IVideoRepository
{
    private readonly DatabaseContext _databaseContext;

    public VideoRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<List<VideoModel>> GetVideosAsync()
    {
        return await _databaseContext.Videos.ToListAsync();
    }

    public async Task<VideoModel?> GetVideoByIdAsync(Guid id)
    {
        var video = await _databaseContext.Videos.FirstOrDefaultAsync(v => v.Id == id);
        return video;
    }

    public async Task AddVideoAsync(VideoModel video)
    {
        await _databaseContext.Videos.AddAsync(video);
        await _databaseContext.SaveChangesAsync();
    }
}
