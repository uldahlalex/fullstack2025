using System.Text.Json;
using Api.Rest;
using Api.Websocket;
using Api.Websocket.Documentation;
using Application;
using Application.Models;
using Fleck;
using Infrastructure.Mqtt;
using Infrastructure.Postgres;
using Infrastructure.Websocket;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Startup.Extensions;

namespace Startup;

// public class D;
//
// public class Mock : IWebSocketService<D>
// {
//     public D RegisterConnection(D connection)
//     {
//         return connection;
//     }
//
//     public D OnClose(D ws)
//     {
//         return ws;
//     }
// }

public class Program
{
    public static async Task Main()
    {
        var builder = WebApplication.CreateBuilder();

        ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

        var app = builder.Build();

        await ConfigureMiddleware(app);

        await app.RunAsync();
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddAppOptions(configuration, environment);
        services.AddSingleton<IProxyConfig, ProxyConfig>();

        services.RegisterApplicationServices<IWebSocketConnection>();

        services.AddDataSourceAndRepositories();
        services.AddWebsocketInfrastructure();
        services.RegisterMqttInfrastructure();
        services.RegisterWebsocketApiServices();

        services.RegisterRestApiServices();

        services.AddOpenApiDocument(conf =>
        {
            conf.DocumentProcessors.Add(new AddAllDerivedTypesProcessor());
            conf.DocumentProcessors.Add(new AddStringConstantsProcessor());
        });
    }

    public static async Task ConfigureMiddleware(WebApplication app)
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
        
        // Define ports
        const int restPort = 5000;
        const int wsPort = 8181;
        var publicPort = int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "8080");

        // Configure the REST API endpoint
        app.Urls.Add($"http://0.0.0.0:{restPort}");

        // Configure proxy to handle both REST and WebSocket traffic
        app.Services.GetRequiredService<IProxyConfig>()
            .StartProxyServer(publicPort, restPort, wsPort);

        // Configure the APIs
        app.ConfigureRestApi();
        await app.ConfigureWebsocketApi(wsPort);
        app.ConfigureMqtt();


        app.MapGet("Acceptance", () => "Accepted");

        app.UseOpenApi();
        app.MapScalarApiReference();
        app.GenerateTypeScriptClient("v1").GetAwaiter().GetResult();
    }
}