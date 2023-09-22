using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpVids.Models;

public sealed class VideoModel
{
	[Key]
	public required Guid Id { get; set; }

	[ForeignKey(nameof(Creator))]
	public required Guid CreatorId { get; set; }
	public UserModel Creator {  get; set; }

	public required string Title { get; set; }
	public required string Description { get; set; }
	public required DateTimeOffset UploadDate { get; set; }

	[Url]
	public required string ThumbnailUrl { get; set; }

	public required List<byte> VideoBytes { get; set; }

	public List<CommentModel> Comments { get; set; }
}
