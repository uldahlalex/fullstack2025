using System.Text.Json;
using Api.Websocket;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Fleck;
using Infrastructure.Postgres;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PgCtx;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;


namespace Startup.Tests;

public class ApiTestBase(ITestOutputHelper outputHelper, ApiTestBaseConfig? apiTestBaseConfig = null)
    : WebApplicationFactory<Program>
{
    private readonly ApiTestBaseConfig _apiTestBaseConfig = apiTestBaseConfig ?? new ApiTestBaseConfig();
    private readonly PgCtxSetup<MyDbContext> _pgCtxSetup = new();  
    private readonly TaskCompletionSource<int> _wsPortInitialized = new();

    

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Trace);
            logging.AddXUnit(outputHelper);
        });
        
        builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables(prefix: "APPOPTIONS__");
        });
        builder.ConfigureServices(ConfigureTestServices);
    }

  


    private void ConfigureTestServices(WebHostBuilderContext context, IServiceCollection services)
    {
        var configuration = context.Configuration;
        outputHelper.WriteLine($"Loaded configuration: {JsonSerializer.Serialize(configuration.GetSection("AppOptions").Get<AppOptions>())}");
        if (_apiTestBaseConfig.MockRelationalDatabase)
        {
            RemoveExistingService<DbContextOptions<MyDbContext>>(services);
            var mock = new Mock<MyDbContext>().Object;
            services.AddScoped<MyDbContext>(sp => mock);
        }
        else
        {
            RemoveExistingService<DbContextOptions<MyDbContext>>(services);
            services.AddDbContext<MyDbContext>(opt =>
            {
                opt.UseNpgsql(_pgCtxSetup._postgres.GetConnectionString());
                opt.EnableSensitiveDataLogging();
                opt.LogTo(_ => { });
            });
        }


        if (_apiTestBaseConfig.MockMqtt)
        {
            RemoveExistingService<IMqttClientService>(services);
            var mockMqttClientService = new Mock<IMqttClientService>();
            services.AddSingleton(mockMqttClientService.Object);
        }

        if (_apiTestBaseConfig.MockProxyConfig)
        {
            RemoveExistingService<IProxyConfig>(services);
            services.AddSingleton<IProxyConfig, MockProxyConfig>();
        }

        if (_apiTestBaseConfig.UseCustomSeeder)
        {
            RemoveExistingService<ISeeder>(services);
            services.AddSingleton<ISeeder, TestSeeder>();
        }
        if (_apiTestBaseConfig.MockWebSocketService)
        {
            var mockWsService = new Mock<IWebSocketService<IWebSocketConnection>>();
            services.AddSingleton(mockWsService.Object);
        }
    }

    private void RemoveExistingService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
            services.Remove(descriptor);
    }

  
}