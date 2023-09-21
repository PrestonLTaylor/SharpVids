using Bogus;
using SharpVids.Models;

namespace SharpVids.Data.Generators;

public sealed class FakeVideoGenerator
{
	public FakeVideoGenerator()
	{
		_faker.RuleFor(u => u.Id, f => Guid.NewGuid())
			  // TODO: Add constructor argument for a user if we need it
			  .RuleFor(u => u.CreatorId, f => Guid.NewGuid())
			  .RuleFor(u => u.Title, f => f.Hacker.Phrase())
			  .RuleFor(u => u.Description, f => f.Hacker.Phrase())
			  .RuleFor(u => u.UploadDate, f => f.Date.PastOffset().UtcDateTime)
			  // TODO: Proper aspect ratio for when we display thumbnails
			  .RuleFor(u => u.ThumbnailUrl, f => f.Image.PicsumUrl(80, 80));
			  // TODO: Maybe generate a fake video/premade video
	}

	public IEnumerable<VideoModel> GenerateForever()
	{
		return _faker.GenerateForever();
	}

	private readonly Faker<VideoModel> _faker = new();
}
