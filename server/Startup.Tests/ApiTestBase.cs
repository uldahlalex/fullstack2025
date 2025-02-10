using Api.Websocket;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Interfaces.Infrastructure.Websocket;
using Fleck;
using Infrastructure.Postgres;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
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


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {          
        builder.UseEnvironment("Testing");

        builder.ConfigureLogging(logging =>
             {
                 logging.ClearProviders();
                 logging.SetMinimumLevel(LogLevel.Trace);
                 logging.AddXUnit(outputHelper);
             });
        builder.ConfigureServices(ConfigureTestServices);
    
      
           
        // Use localhost with dynamic port for tests
        builder.UseUrls("http://127.0.0.1:0");
        
        // Configure the application
        builder.Configure(async app =>
        {
            if (app is WebApplication webApp)
            {
                // Configure your middleware and routing first
                webApp.UseRouting();
                
                // Start the WebSocket server
                await webApp.ConfigureWebsocketApi();
                
                // Get the actual port being used
                var serverAddress = webApp.Urls.Select(url => new Uri(url)).First();
                outputHelper.WriteLine($"Test server running on port: {serverAddress.Port}");
            }
        });
    }
    

    private void ConfigureTestServices(WebHostBuilderContext context, IServiceCollection services)
    {
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

public class ApiTestBaseConfig
{
    /// <summary>
    ///     Defaults to false
    /// </summary>
    public bool MockRelationalDatabase { get; set; } = false;

    /// <summary>
    ///     Defaults to false
    /// </summary>
    public bool MockMqtt { get; set; } = false;

    /// <summary>
    ///     Defaults to false
    /// </summary>
    public bool MockWebSocketService { get; set; } = false;

    /// <summary>
    ///     Defaults to true
    /// </summary>
    public bool MockProxyConfig { get; set; } = true;

    /// <summary>
    ///     Defaults to true
    /// </summary>
    public bool UseCustomSeeder { get; set; } = true;
}