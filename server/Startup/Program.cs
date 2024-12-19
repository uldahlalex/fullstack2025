using System.Text.Json;
using Api.Mqtt;
using Api.Rest;
using Api.Websocket;
using Application.Extensions;
using Application.Models;
using Infrastructure.Mqtt;
using Infrastructure.Postgres;
using Infrastructure.Websocket;
using Microsoft.Extensions.Options;
using Startup.Extensions;

namespace Startup;

public class Program
{
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();

        ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

        var app = builder.Build();

        ConfigureMiddleware(app);

        app.Run();
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddAppOptions(configuration, environment);
        services.AddSingleton<IProxyConfig, ProxyConfig>();

        services.AddDataSourceAndRepositories();

        services.AddWebsocketInfrastructure();
        services.AddMqttInfrastructure();

        services.AddApplicationServices();

        services.AddDependenciesForRestApi();
        services.AddDependenciesForRealtimeApi();
        services.AddDependenciesForMqttApi();
    }

    public static void ConfigureMiddleware(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var options = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<AppOptions>>();
            Console.WriteLine(JsonSerializer.Serialize(options.CurrentValue));
            if (options.CurrentValue.Seed)
            {
                var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
                seeder.Seed().Wait();
            }
        }

        app.Urls.Clear();
        const int restPort = 5000;
        const int wsPort = 8181;
        var publicPort = int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "8080");
        app.Urls.Add($"http://0.0.0.0:{restPort}"); 
        app.Services.GetRequiredService<IProxyConfig>().StartProxyServer(publicPort, restPort, wsPort);
        app.AddMiddlewareForRestApi();
        app.AddMiddlewareForRealtimeApi();
        app.AddMiddlewareForMqttApi();
        app.MapGet("Acceptance", () => "Accepted");
    }
}