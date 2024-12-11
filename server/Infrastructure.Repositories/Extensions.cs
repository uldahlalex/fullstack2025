using infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using service.Interfaces;

namespace Infrastructure.Repositories;

public static class Extensions
{
    public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services, string databaseUrl)
    {
        services.AddDbContext<MyDbContext>(options =>
        {
            Console.WriteLine("Connecting to DB at: "+databaseUrl);
            options.EnableSensitiveDataLogging();
            options.UseNpgsql(databaseUrl);
        });


        services.AddScoped<IRepoLogic, Repo>();
        services.AddScoped<Seeder>();
        return services;
    }

  
}