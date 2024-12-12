using infrastructure;
using Infrastructure.Repositories.Postgresql.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using PgCtx;
using service;
using service.Interfaces;

namespace Infrastructure.Repositories;

public static class Extensions
{
    public static IServiceCollection AddDataSourceAndRepositories(this IServiceCollection services)
    {
        
        try
        {
            services.AddDbContext<MyDbContext>((serviceProvider, options) =>
            {
                Console.WriteLine("Connecting to DB at "+ serviceProvider.GetRequiredService<IOptionsMonitor<AppOptions>>().CurrentValue.DbConnectionString);
                var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AppOptions>>();
                options.EnableSensitiveDataLogging();
                options.UseNpgsql(optionsMonitor.CurrentValue.DbConnectionString);
            });
            //Test DB connection
         
            var result = services.BuildServiceProvider().GetRequiredService<MyDbContext>().Database.ExecuteSql($"Select 'test';");
            Console.WriteLine(result);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error connecting to DB: " + e.Message + " " + e.StackTrace);
            Console.WriteLine("Starting DB in testcontainer instead");
            var pgCtxSetup = new PgCtxSetup<MyDbContext>();
            services.AddDbContext<MyDbContext>((serviceProvider, options) =>
            {
                var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AppOptions>>();
                optionsMonitor.CurrentValue.DbConnectionString = pgCtxSetup._postgres.GetConnectionString();
                options.EnableSensitiveDataLogging();
                options.UseNpgsql(optionsMonitor.CurrentValue.DbConnectionString);
            });
        }




        services.AddScoped<IDataRepository, Repo>();
        services.AddScoped<Seeder>();
        return services;
    } 

  
}