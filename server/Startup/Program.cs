using System.Text.Json;
using Api.Realtime;
using Api.Rest;
using infrastructure;
using Infrastructure.Repositories;
using service;

namespace Startup;

public static class Program
{
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();

        var options = builder.AddAppOptions();
        Console.WriteLine("Starting with options: "+JsonSerializer.Serialize(options));

        builder.Services.AddDataSourceAndRepositories(options.DbConnectionString);
        builder.Services.AddApplicationServices();
        builder.AddDependenciesForRestApi();
        builder.AddDependenciesForRealtimeApi();
        builder.WebHost.UseUrls("http://*:5000");

        var app = builder.Build();

        new ProxyConfig().StartProxyServer();


        app.AddMiddlewareForRestApi();
        app.AddMiddlewareForRealtimeApi();

        using (var scope = app.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<MyDbContext>().Database.EnsureCreated();
        }

        app.Run(Environment.GetEnvironmentVariable("PORT")??"8080");
    }
}