using core;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories;

public static class Extensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepoLogic, Repo>();
        return services;
    }
}
