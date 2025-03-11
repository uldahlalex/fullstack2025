

### program startup:

```cs
// ./server/Startup/Program.cs

using Api.Rest;
using Api.Websocket;
using Application;
using Application.Models;
using Infrastructure.Mqtt;
using Infrastructure.Postgres;
using Infrastructure.Websocket;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Web;
using Scalar.AspNetCore;
using Startup.Documentation;
using Startup.Proxy;

namespace Startup;

public class Program
{
    public static async Task Main()
    {
        var logger = LogManager.Setup()
            .LoadConfigurationFromAppSettings()
            .GetCurrentClassLogger();
        var builder = WebApplication.CreateBuilder();


        builder.Logging.ClearProviders();
        builder.Host.UseNLog();
        ConfigureServices(builder.Services, builder.Configuration);
        var app = builder.Build();
        await ConfigureMiddleware(app);
        await app.RunAsync();
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAppOptions(configuration);

        services.RegisterApplicationServices();

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
        app.GenerateTypeScriptClient("/../../client/src/generated-client.ts").GetAwaiter().GetResult();
    }
}
```

### Scaffolding:

```bash
# ./server/Infrastructure.Postgres.Scaffolding/scaffold.sh

!/bin/bash
#the below will read the the CONN_STR from .env file - you may change this to your connection string
set -a
source .env
set +a

dotnet ef dbcontext scaffold $CONN_STR   Npgsql.EntityFrameworkCore.PostgreSQL  --output-dir ../Application/Models/Entities   --context-dir .   --context MyDbContext --no-onconfiguring  --namespace Application.Models.Entities --context-namespace  Infrastructure.Postgres.Scaffolding --schema surveillance --force 

```
