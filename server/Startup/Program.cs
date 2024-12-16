using Api.Realtime;
using Api.Rest;
using Infrastructure.Mqtt;
using Infrastructure.Repositories;
using Infrastructure.Websocket;
using Microsoft.Extensions.Options;
using service;
using service.Extensions;
using Startup.Extensions;

namespace Startup;

public class Program
{
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        ConfigureMiddleware(app);

        //var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        //var url = $"http://0.0.0.0:{port}";
        app.Run(
        //    url
        );
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAppOptions(configuration);
        services.AddSingleton<IProxyConfig, ProxyConfig>();
        services.AddDataSourceAndRepositories();
        services.AddWebsocketInfrastructure();
        services.AddMqttInfrastructure();

        services.AddApplicationServices();

        services.AddDependenciesForRestApi();
        services.AddDependenciesForRealtimeApi();
    }

    public static void ConfigureMiddleware(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var options = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<AppOptions>>();
            if (options.CurrentValue.Seed)
            {
                var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
                seeder.Seed().Wait();
            }
        }
        
        app.Services.GetRequiredService<IProxyConfig>().StartProxyServer();
        app.AddMiddlewareForRestApi();
        app.AddMiddlewareForRealtimeApi();
        app.MapGet("Acceptance", () => "Accepted");
    }
}