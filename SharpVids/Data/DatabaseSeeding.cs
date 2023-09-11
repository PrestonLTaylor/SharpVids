using Bogus;
using Microsoft.AspNetCore.Identity;
using SharpVids.Data.Generators;
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

		var fakeUserGenerator = new FakeUserGenerator();
		var fakeUsers = fakeUserGenerator.GenerateForever().Take(100);
		foreach (var fakeUser in fakeUsers)
		{
			const string fakePassword = "Password12/";
			await userManager.CreateAsync(fakeUser, fakePassword);
		}
	}
}
