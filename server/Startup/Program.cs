using Api.Realtime;
using Api.Rest;
using Api.Tests;
using Infrastructure.Mqtt;
using Infrastructure.Repositories;
using Infrastructure.Websocket;
using service.Extensions;
using Startup.Extensions;

namespace Startup;

public  class Program
{
    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();

        var options = builder.AddAppOptions();
        builder.Services.AddSingleton<IProxyConfig, ProxyConfig>();
        builder.Services.AddDataSourceAndRepositories();
        builder.Services.AddWebsocketInfrastructure();
        builder.Services.AddMqttInfrastructure();

        builder.Services.AddApplicationServices();

        builder.AddDependenciesForRestApi();
        builder.AddDependenciesForRealtimeApi();

        // builder.WebHost.UseUrls("http://*:5000");

        var app = builder.Build();

        app.Services.GetRequiredService<IProxyConfig>().StartProxyServer();
        app.AddMiddlewareForRestApi();
        app.AddMiddlewareForRealtimeApi();

        app.MapGet("Acceptance", () =>
        {   
            return "Acceptance";

        });


        if (options.Seed)
        {
            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
            seeder.Seed().Wait();
        }

        app.Run();
    }
}