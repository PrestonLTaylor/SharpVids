using SharpVids.Models;
using System.Security.Claims;

namespace SharpVids.Data.Repositories;

public interface IUserRepository
{
	public Task<List<UserModel>> GetUsersAsync();
	public Task<UserModel?> GetUserByIdAsync(Guid id);
	public Task<UserModel?> GetUserByClaimAsync(ClaimsPrincipal claim);
}
