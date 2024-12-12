using infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using PgCtx;
using service.Interfaces;
using service.Types;

namespace Infrastructure.Repositories;

public static class Extensions
{
    public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services)
    {
        services.AddDbContext<MyDbContext>((serviceProvider, options) =>
        {
            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AppOptions>>();
            Console.WriteLine("Connecting to DB at: "+optionsMonitor.CurrentValue.DbConnectionString);
            try
            {
                var conn = new NpgsqlConnection(optionsMonitor.CurrentValue.DbConnectionString);
                conn.Open();
                _ = new NpgsqlCommand("SELECT 'Hello, world!'", new NpgsqlConnection());
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error connecting to DB: " + e.Message + " " + e.StackTrace);
                Console.WriteLine("Starting DB in testcontainer instead");
                var pgCtxSetup = new PgCtxSetup<MyDbContext>();
                optionsMonitor.CurrentValue.DbConnectionString = pgCtxSetup._postgres.GetConnectionString();
            }
            
            options.EnableSensitiveDataLogging();
            options.UseNpgsql(optionsMonitor.CurrentValue.DbConnectionString);
        });


        services.AddScoped<IRepoLogic, Repo>();
        services.AddScoped<Seeder>();
        return services;
    }

  
}