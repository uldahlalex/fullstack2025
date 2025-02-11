using Api.Rest;
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

public class Program
{
    public static async Task Main()
    {
        var builder = WebApplication.CreateBuilder();
        ConfigureServices(builder.Services, builder.Configuration);
        var app = builder.Build();
        await ConfigureMiddleware(app);
        await app.RunAsync();
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAppOptions(configuration);

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


        app.Urls.Clear();
        app.Urls.Add($"http://0.0.0.0:{appOptions.REST_PORT}");
        app.Services.GetRequiredService<IProxyConfig>()
            .StartProxyServer(appOptions.PORT, appOptions.REST_PORT, appOptions.WS_PORT);

        app.ConfigureRestApi();
        await app.ConfigureWebsocketApi(appOptions.WS_PORT);
        app.ConfigureMqtt();

        app.MapGet("Acceptance", () => "Accepted");

        app.UseOpenApi();
        app.MapScalarApiReference();
        await app.GenerateTypeScriptClient("v1");
    }
}