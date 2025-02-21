using Api;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Websocket;

public static class Extensions
{
    public static IServiceCollection AddWebsocketInfrastructure(this IServiceCollection services)
    {
        var appOptions = services
            .BuildServiceProvider()
            .GetRequiredService<IOptionsMonitor<AppOptions>>()
            .CurrentValue;

        var redisConfig = new ConfigurationOptions
        {
            AbortOnConnectFail = false,
            ConnectTimeout = 5000,
            SyncTimeout = 5000,
            Ssl = true,
            DefaultDatabase = 0,
            ConnectRetry = 5,
            ReconnectRetryPolicy = new ExponentialRetry(5000),
            EndPoints = { { appOptions.REDIS_HOST, 6379 } },
            User = appOptions.REDIS_USERNAME,
            Password = appOptions.REDIS_PASSWORD
        };

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var multiplexer = ConnectionMultiplexer.Connect(redisConfig);
            return multiplexer;
        });
        services.AddSingleton<IConnectionManager, RedisConnectionManager>();
        return services;
    }
}