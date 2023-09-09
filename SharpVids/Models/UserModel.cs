using Microsoft.AspNetCore.Identity;

namespace SharpVids.Models;

public sealed class UserModel : IdentityUser<Guid>
{
	// TODO: Put a default profile picture url here
	public required string ProfilePictureUrl { get; set; } = "";

	public required DateTimeOffset RegistrationDate { get; set; } = DateTimeOffset.UtcNow;

	public required List<VideoModel> Videos { get; set; }
	public required List<CommentModel> Comments { get; set; }
}
