using Microsoft.EntityFrameworkCore;
using SharpVids.Models;

namespace SharpVids.Data.Repositories;

public sealed class UserRepository : IUserRepository
{
	private readonly DatabaseContext _databaseContext;

	public UserRepository(DatabaseContext databaseContext)
	{
		_databaseContext = databaseContext;
	}

	public async Task<List<UserModel>> GetUsersAsync()
	{
		return await _databaseContext.Users.ToListAsync();
	}

	public async Task<UserModel?> GetUserByIdAsync(Guid id)
	{
		return await _databaseContext.Users.FirstOrDefaultAsync(u => u.Id == id);
	}
}
