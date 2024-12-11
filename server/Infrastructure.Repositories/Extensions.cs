using infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using service.Interfaces;

namespace Infrastructure.Repositories;

public static class Extensions
{
    public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services, string connString)
    {
        services.AddDbContext<MyDbContext>(options =>
        {
            options.EnableSensitiveDataLogging();
            options.UseNpgsql(connString);
        });


        services.AddScoped<IRepoLogic, Repo>();
        return services;
    }
}