using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharpVids.Models;

namespace SharpVids.Data;

public sealed class DatabaseContext : IdentityDbContext<UserModel, IdentityRole<Guid>, Guid>
{
	private readonly IConfiguration _configuration;

	public DatabaseContext(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		string connectionString = _configuration.GetValue<string>("POSTGRESQLCONNSTR_DefaultConnection")
			?? throw new InvalidOperationException("Connection string 'POSTGRESQLCONNSTR_DefaultConnection' is not present.");
		optionsBuilder.UseNpgsql(connectionString);
	}

	public DbSet<VideoModel> Videos { get; set; }
	public DbSet<CommentModel> Comments { get; set; }
}
