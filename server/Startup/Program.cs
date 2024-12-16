using System.Text.Json;
using Api.Rest;
using Api.Websocket;
using Application;
using Application.Extensions;
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

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddAppOptions(configuration, environment);
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
            Console.WriteLine(JsonSerializer.Serialize(options.CurrentValue));
            if (options.CurrentValue.Seed)
            {
                var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
                var delete = options.CurrentValue.ASPNETCORE_ENVIRONMENT.Equals("Development");
                seeder.Seed(delete).Wait();
            }
        }
        
        app.Services.GetRequiredService<IProxyConfig>().StartProxyServer();
        app.AddMiddlewareForRestApi();
        app.AddMiddlewareForRealtimeApi();
        app.MapGet("Acceptance", () => "Accepted");
    }
}