﻿namespace SharpVids.Data.Repositories;

public static class RepositoryServiceInstaller
{
	static public IServiceCollection AddRepositories(this IServiceCollection services)
	{
		services.AddTransient<IUserRepository, UserRepository>();
		services.AddTransient<IVideoRepository, VideoRepository>();
		return services;
	}
}
