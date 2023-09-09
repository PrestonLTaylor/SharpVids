using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharpVids.Models;

public sealed class CommentModel
{
	[Key]
	public Guid Id { get; set; }

	[ForeignKey(nameof(Video))]
	public Guid VideoId { get; set; }
	public required VideoModel Video { get; set; }

	[ForeignKey(nameof(Commenter))]
	public Guid CommenterId { get; set; }
	public required UserModel Commenter { get; set; }

	public required string Comment { get; set; }
	public required DateTimeOffset CommentDate { get; set; }
}
