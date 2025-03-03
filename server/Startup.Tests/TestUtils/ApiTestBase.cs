﻿using Api;
using Api.Websocket.EventHandlers.ClientEventDtos;
using Application.Interfaces.Infrastructure.Mqtt;
using Fleck;
using Infrastructure.Postgres;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit;
using NUnit.Framework;
using PgCtx;
using Startup.Proxy;
using WebSocketBoilerplate;


namespace Startup.Tests.TestUtils;

public class ApiTestBase(ApiTestBaseConfig? apiTestBaseConfig = null)
    : WebApplicationFactory<Program>
{
    private readonly ApiTestBaseConfig _apiTestBaseConfig = apiTestBaseConfig ?? new ApiTestBaseConfig();
    private readonly PgCtxSetup<MyDbContext> _pgCtxSetup = new();

    
    public ILogger<ConnectionWithWsClient> _logger;
    public HttpClient _httpClient;
    public MyDbContext _dbContext;
    public IConnectionManager _connectionManager;
    public string _wsClientId;
    public WsRequestClient _wsClient;
    public IServiceScope _scope;
    


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(ConfigureTestServices);
        
        
    }


    private void ConfigureTestServices(WebHostBuilderContext context, IServiceCollection services)
    {
        if (_apiTestBaseConfig.UseTestContainer)
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
            var mockProxy = new Mock<IProxyConfig>();
            services.AddSingleton(mockProxy.Object);
        }

        if (_apiTestBaseConfig.UseCustomSeeder)
        {
            RemoveExistingService<ISeeder>(services);
            services.AddSingleton<ISeeder, TestEnvironmentSeeder>();
        }

        if (_apiTestBaseConfig.MockWebSocketService)
        {
            var mockWsService = new Mock<IConnectionManager>();
            services.AddSingleton(mockWsService.Object);
        }
    }

    private void RemoveExistingService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
            services.Remove(descriptor);
    }
    
    [SetUp]
    public async Task Setup()
    {
        _httpClient = CreateClient();

        //Singletons
        _logger = Services.GetRequiredService<ILogger<ConnectionWithWsClient>>();
        _connectionManager =  Services.GetRequiredService<IConnectionManager>();

        //Scoped services
        using var scope = Services.CreateScope();
        {
            _scope = Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<MyDbContext>();
        }
        
        var wsPort = Environment.GetEnvironmentVariable("PORT");
        if (string.IsNullOrEmpty(wsPort)) throw new Exception("Environment variable PORT is not set");
        _wsClientId = Guid.NewGuid().ToString();
        var url = "ws://localhost:" + wsPort + "?id=" + _wsClientId;
        _wsClient = new WsRequestClient(
            new[] { typeof(ClientWantsToEchoDto).Assembly },
            url
        );
        await _wsClient.ConnectAsync();
        await Task.Delay(1000);
    }
}