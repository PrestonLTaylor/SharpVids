using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharpVids.Models;
using System.Security.Claims;

namespace SharpVids.Data.Repositories;

public sealed class UserRepository : IUserRepository
{
	private readonly DatabaseContext _databaseContext;
	private readonly UserManager<UserModel> _userManager;

	public UserRepository(DatabaseContext databaseContext, UserManager<UserModel> userManager)
	{
		_databaseContext = databaseContext;
		_userManager = userManager;
	}

	public async Task<List<UserModel>> GetUsersAsync()
	{
		return await _databaseContext.Users.ToListAsync();
	}

	public async Task<UserModel?> GetUserByIdAsync(Guid id)
	{
		return await _databaseContext.Users.FirstOrDefaultAsync(u => u.Id == id);
	}

	public async Task<UserModel?> GetUserByClaimAsync(ClaimsPrincipal claim)
    {
		return await _userManager.GetUserAsync(claim);

    }
}
