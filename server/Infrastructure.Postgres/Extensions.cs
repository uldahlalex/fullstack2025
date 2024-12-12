using infrastructure;
using Infrastructure.Repositories.Postgresql.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PgCtx;
using service;
using service.Interfaces.Infrastructure.Data;

namespace Infrastructure.Repositories;

public static class Extensions
{
    public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AppOptions>>();
        var connectionString = optionsMonitor.CurrentValue.DbConnectionString;

        // Create a DbContextOptionsBuilder and configure it
        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseNpgsql(connectionString);

        // Try to create a DbContext and open a connection
        try
        {
            using (var context = new MyDbContext(optionsBuilder.Options))
            {
                context.Database.OpenConnection();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error connecting to DB: " + e.Message + " " + e.StackTrace);
            Console.WriteLine("Starting DB in test container instead");

            var pgCtxSetup = new PgCtxSetup<MyDbContext>();
            connectionString = pgCtxSetup._postgres.GetConnectionString();

            // Reconfigure the optionsBuilder to use the new connection string
            optionsBuilder.UseNpgsql(connectionString);
        }

        // Add the DbContext with the configured options to the service collection
        services.AddDbContext<MyDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.EnableSensitiveDataLogging();
        });

        services.AddScoped<IDataRepository, Repo>();
        services.AddScoped<Seeder>();

        return services;
    }
}