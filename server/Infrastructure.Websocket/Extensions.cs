using Api;
using Api.WebSockets;
using Application.Models;
using Fleck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WebSocketBoilerplate;

namespace Infrastructure.Websocket;

public static class Extensions
{
    public static IServiceCollection AddWebsocketInfrastructure(this IServiceCollection services)
    {
        var appOptions = services
            .BuildServiceProvider()
            .GetRequiredService<IOptionsMonitor<AppOptions>>()
            .CurrentValue;
        services.AddSingleton<IConnectionManager, WebSocketConnectionManager>();
        return services;
    }
}