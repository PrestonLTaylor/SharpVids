using Microsoft.AspNetCore.Mvc;
using Moq;
using SharpVids.Controllers;
using SharpVids.Data.Generators;
using SharpVids.Data.Repositories;
using SharpVids.Models;

namespace SharpVids.Test;

internal sealed class UserControllerTests
{
	[Test]
	public async Task Users_ReturnsEveryRegisteredUser()
	{
		// Arrange
		var fakeUsers = GenerateFakeUsers();
		_userRepositoryMock.Setup(r => r.GetUsersAsync()).ReturnsAsync(fakeUsers);

		// Act
		var userController = new UserController(_userRepositoryMock.Object);
		var viewResult = await userController.Users() as ViewResult;
		var result = viewResult?.Model as List<UserModel>;

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Has.Count.EqualTo(fakeUsers.Count));
	}

	[Test]
	public async Task Users_ReturnsAnEmptyList_WithNoRegisteredUsers()
	{
		// Arrange
		_userRepositoryMock.Setup(r => r.GetUsersAsync()).ReturnsAsync(new List<UserModel>());

		// Act
		var userController = new UserController(_userRepositoryMock.Object);
		var viewResult = await userController.Users() as ViewResult;
		var result = viewResult?.Model as List<UserModel>;

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Any(), Is.False);
	}

	[Test]
	public async Task Profile_ReturnsSpecifiedUser_WhenProvidedId()
	{
		// Arrange
		var fakeUsers = GenerateFakeUsers();
		var specificUser = fakeUsers.First();
		_userRepositoryMock.Setup(r => r.GetUserByIdAsync(specificUser.Id)).ReturnsAsync(specificUser);

		// Act
		var userController = new UserController(_userRepositoryMock.Object);
		var viewResult = await userController.Profile(specificUser.Id) as ViewResult;
		var result = viewResult?.Model as UserModel;

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Id, Is.EqualTo(specificUser.Id));
	}

	[Test]
	public async Task Profile_NotFound_WhenProvidedInvalidId()
	{
		// Arrange
		_userRepositoryMock.Setup(r => r.GetUserByIdAsync(It.IsAny<Guid>())).ReturnsAsync((UserModel?)null);

		// Act
		var userController = new UserController(_userRepositoryMock.Object);
		var notFoundResult = await userController.Profile(Guid.NewGuid()) as NotFoundResult;

		// Assert
		Assert.That(notFoundResult, Is.Not.Null);
	}

	static private List<UserModel> GenerateFakeUsers()
	{
		FakeUserGenerator fakeUserGenerator = new();
		int numberOfUsers = Random.Shared.Next(1, 100);
		return fakeUserGenerator.GenerateForever().Take(numberOfUsers).ToList();
	}

	private readonly Mock<IUserRepository> _userRepositoryMock = new();
}
