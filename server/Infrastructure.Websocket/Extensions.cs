using Application.Interfaces.Infrastructure.Websocket;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Websocket;

public static class Extensions
{
    public static IServiceCollection AddWebsocketInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IState, State>();
        services.AddSingleton<IConnectionRegistry, ConnectionRegistry>();
        services.AddSingleton<IConnectionCreator, WebSocketConnectionCreator>();
        services.AddScoped<IWebsocketClientMessager, WebsocketClientMessager>();
        return services;
    }
}