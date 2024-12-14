using Api.Realtime;
using Api.Rest;
using Infrastructure.Mqtt;
using Infrastructure.Repositories;
using Infrastructure.Websocket;
using service;
using service.Extensions;
using Startup.Extensions;

namespace Startup;

public class Program
{
    private static HashSet<Type> _eventHandlers;
    private static AppOptions _appOptions;


    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        
        ConfigureServices(builder.Services, builder.Configuration);
        
        var app = builder.Build();
        
        ConfigureMiddleware(app);
        
        // var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        // var url = $"http://0.0.0.0:{port}";
        app.Run(
            // url
            );
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        _appOptions = services.AddAppOptions(configuration);
        services.AddSingleton<IProxyConfig, ProxyConfig>();
        services.AddDataSourceAndRepositories();
        services.AddWebsocketInfrastructure();
        services.AddMqttInfrastructure();

        services.AddApplicationServices();

        services.AddDependenciesForRestApi();
        _eventHandlers = services.AddDependenciesForRealtimeApiReturnEventHandlers();
    }

    public static void ConfigureMiddleware( WebApplication app)
    {
        if (_appOptions.Seed)
        {
            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
            seeder.Seed().Wait();
        }

        app.Services.GetRequiredService<IProxyConfig>().StartProxyServer();
        app.AddMiddlewareForRestApi();
        app.AddMiddlewareForRealtimeApi(_eventHandlers);

        app.MapGet("Acceptance", () => "Accepted");
    }
}