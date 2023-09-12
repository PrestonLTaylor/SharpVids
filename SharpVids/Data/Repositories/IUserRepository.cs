using SharpVids.Models;

namespace SharpVids.Data.Repositories;

public interface IUserRepository
{
	public Task<List<UserModel>> GetUsersAsync();
	public Task<UserModel?> GetUserByIdAsync(Guid id);
}
