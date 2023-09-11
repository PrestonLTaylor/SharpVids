using Bogus;
using SharpVids.Models;

namespace SharpVids.Data.Generators;

public sealed class FakeUserGenerator
{
	public FakeUserGenerator()
	{
		_faker.RuleFor(u => u.Id, f => Guid.NewGuid())
			  .RuleFor(u => u.UserName, f => f.Internet.UserName())
			  .RuleFor(u => u.Email, f => f.Internet.Email())
			  .RuleFor(u => u.ProfilePictureUrl, f => f.Image.PicsumUrl(80, 80))
			  .RuleFor(u => u.RegistrationDate, f => f.Date.PastOffset().UtcDateTime);
	}

	public IEnumerable<UserModel> GenerateForever()
	{
		return _faker.GenerateForever();
	}

	private readonly Faker<UserModel> _faker = new();
}
