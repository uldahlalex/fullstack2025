using Application.Interfaces.Infrastructure.Postgres;
using Application.Models;
using Infrastructure.Postgres.Postgresql.Data;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PgCtx;

namespace Infrastructure.Postgres;

public static class Extensions
{
    public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AppOptions>>();
        var connectionString = optionsMonitor.CurrentValue.DbConnectionString;

        // var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
        // optionsBuilder.EnableSensitiveDataLogging();
        // optionsBuilder.UseNpgsql(connectionString);
        //
        // try
        // {
        //     using (var context = new MyDbContext(optionsBuilder.Options))
        //     {
        //         context.Database.OpenConnection();
        //     }
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine("Error connecting to DB: " + e.Message);
        //     Console.WriteLine(e.InnerException);
        //     Console.WriteLine(e.StackTrace);
        //     Console.WriteLine("Starting DB in test container instead");
        //
        //     if (optionsMonitor.CurrentValue.ASPNETCORE_ENVIRONMENT.Equals("Production"))
        //         throw;
        //     var pgCtxSetup = new PgCtxSetup<MyDbContext>();
        //     connectionString = pgCtxSetup._postgres.GetConnectionString();
        //     optionsMonitor.CurrentValue.DbConnectionString = connectionString;
        //     optionsBuilder.UseNpgsql(connectionString);
        // }

        services.AddDbContext<MyDbContext>(options =>
        {
            options.UseNpgsql(connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "jerneif"));
            options.EnableSensitiveDataLogging();
        });

        services.AddScoped<IDataRepository, Repo>();
        services.AddScoped<Seeder>();

        return services;
    }
}