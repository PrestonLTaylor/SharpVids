using Microsoft.AspNetCore.Mvc;
using SharpVids.Data.Repositories;

namespace SharpVids.Controllers;

public sealed class UserController : Controller
{
	private readonly IUserRepository _userRepository;

	public UserController(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<IActionResult> Users()
	{
		var users = await _userRepository.GetUsersAsync();
		return View(users);
	}

	public async Task<IActionResult> Profile([FromRoute]Guid id)
	{
		var user = await _userRepository.GetUserByIdAsync(id);
		if (user is null)
		{
			return NotFound();
		}

		return View(user);
	}
}
