using System.Net;
using System.Net.Sockets;
using Api.Rest;
using Api.Websocket;
using Application;
using Application.Models;
using Core.Domain;
using Docker.DotNet;
using Infrastructure.Mqtt;
using Infrastructure.Postgres;
using Infrastructure.Websocket;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSwag.Generation;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Templates;
using Serilog.Templates.Themes;
using Startup.Documentation;
using Startup.Proxy;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Startup;

public class Program
{
    public static async Task Main()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddSuperAwesomeLoggingConfig();
        ConfigureServices(builder.Services, builder.Configuration);
        var app = builder.Build();
        await ConfigureMiddleware(app);
        await app.RunAsync();
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
       var appOptions = services.AddAppOptions(configuration);

        services.RegisterApplicationServices();

        services.AddDataSourceAndRepositories();
        services.AddWebsocketInfrastructure();
        if (!string.IsNullOrEmpty(appOptions.MQTT_BROKER_HOST))
        {
            services.RegisterMqttInfrastructure();
        }
        else
        {
            var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
            logger.LogInformation("No MQTT_BROKER_HOST provided, skipping MQTT configuration (you're probably not doing IoT stuff)");

        }

        services.RegisterWebsocketApiServices();
        services.RegisterRestApiServices();
        services.AddOpenApiDocument(conf =>
        {
            conf.DocumentProcessors.Add(new AddAllDerivedTypesProcessor());
            conf.DocumentProcessors.Add(new AddStringConstantsProcessor());
        });
        services.AddSingleton<IProxyConfig, ProxyConfig>();
    }

    public static async Task ConfigureMiddleware(WebApplication app)
    {
        var appOptions = app.Services.GetRequiredService<IOptionsMonitor<AppOptions>>().CurrentValue;

        using (var scope = app.Services.CreateScope())
        {
            if (appOptions.Seed)
                await scope.ServiceProvider.GetRequiredService<Seeder>().Seed();
        }

        app.Services.GetRequiredService<ILogger<string>>()
            .LogInformation("App starting with app options: " + JsonSerializer.Serialize(appOptions));

        var restPort = PortHelper.FindAvailablePort(5000, 5100);
        var wsPort = PortHelper.FindAvailablePort(8180, 8280);
        app.Urls.Clear();
        app.Urls.Add($"http://0.0.0.0:{restPort}");
        app.Services.GetRequiredService<IProxyConfig>()
            .StartProxyServer(
                publicPort: appOptions.PORT, 
                restPort: restPort,
                wsPort: wsPort
                );
        app.ConfigureRestApi();
        await app.ConfigureWebsocketApi(wsPort);
        if (!string.IsNullOrEmpty(appOptions.MQTT_BROKER_HOST))
        {
            await app.ConfigureMqtt();
        }
        else
        {
            app.Logger.LogInformation("No MQTT_BROKER_HOST provided, skipping MQTT configuration (you're probably not doing IoT stuff)");
        }

        app.MapGet("Acceptance", () => "Accepted");

        app.UseOpenApi(conf =>
        {
            conf.Path = "openapi/v1.json";
        });
        app.MapScalarApiReference();

        var document = await app.Services.GetRequiredService<IOpenApiDocumentGenerator>().GenerateAsync("v1");
        var json = document.ToJson();
        await File.WriteAllTextAsync("openapi.json", json);

        app.GenerateTypeScriptClient("/../../client/src/generated-client.ts").GetAwaiter().GetResult();

    }
}

public static class PortHelper
{
    public static int FindAvailablePort(int startPort = 5000, int endPort = 6000)
    {
        var ipAddress = IPAddress.Parse("0.0.0.0");
        for (int port = startPort; port <= endPort; port++)
        {
            try
            {
                using var testListener = new TcpListener(ipAddress, port);
                testListener.Start();
                testListener.Stop();
                return port;
            }
            catch
            {
                continue;
            }
        }
        throw new Exception($"No available ports found between {startPort} and {endPort}");
    }
}