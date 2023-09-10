using Bogus;
using Microsoft.AspNetCore.Identity;
using SharpVids.Models;

namespace SharpVids.Data;

public static class DatabaseSeeding
{
	static public async Task<IApplicationBuilder> SeedDatabaseAsync(this IApplicationBuilder applicationBuilder)
	{
		using var scope = applicationBuilder.ApplicationServices.CreateScope();
		var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

		await databaseContext.Database.EnsureCreatedAsync();

		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
		await GenerateFakeUsersAsync(databaseContext, userManager);

		// TODO: GenerateFakeVideosAsync()
		// TODO: GenerateFakeCommentsAsync()

		await databaseContext.SaveChangesAsync();

		return applicationBuilder;
	}

	static private async Task GenerateFakeUsersAsync(DatabaseContext databaseContext, UserManager<UserModel> userManager)
	{
		if (databaseContext.Users.Any()) return;

		var userFaker = new Faker<UserModel>()
			.RuleFor(u => u.Id, f => Guid.NewGuid())
			.RuleFor(u => u.UserName, f => f.Internet.UserName())
			.RuleFor(u => u.Email, f => f.Internet.Email())
			.RuleFor(u => u.ProfilePictureUrl, f => f.Image.PicsumUrl(80, 80))
			.RuleFor(u => u.RegistrationDate, f => f.Date.PastOffset().UtcDateTime);

		const int numberOfUsers = 100;
		const string fakePassword = "Password12/";
		for (int i = 0; i < numberOfUsers; ++i)
		{
			var fakeUser = userFaker.Generate();
			await userManager.CreateAsync(fakeUser, fakePassword);
		}
	}
}
