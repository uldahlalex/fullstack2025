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
            // Console.WriteLine("Connecting to DB at: "+databaseUrl);
            // try
            // {
            //
            //     //context.Database.ExecuteSql($"Select 'test';");
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine("Error connecting to DB: " + e.Message + " " + e.StackTrace);
            //     Console.WriteLine("Starting DB in testcontainer instead");
            //     var pgCtxSetup = new PgCtxSetup<MyDbContext>();
            //     Environment.SetEnvironmentVariable("", pgCtxSetup._postgres.GetConnectionString());
            // }todo
            
            options.EnableSensitiveDataLogging();
            options.UseNpgsql(databaseUrl);
        });


        services.AddScoped<IRepoLogic, Repo>();
        services.AddScoped<Seeder>();
        return services;
    }

  
}