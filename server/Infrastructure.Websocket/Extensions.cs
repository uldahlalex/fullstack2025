using Application;
using Application.Interfaces.Infrastructure.Broadcasting;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Websocket;

public static class Extensions
{
    public static IServiceCollection AddWebsocketInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IState, State>();
        services.AddSingleton<IConnectionRegistry, ConnectionRegistry>();
        services.AddSingleton<IConnectionCreator, WebSocketConnectionCreator>();
        services.AddScoped<IBroadcaster, Broadcaster>();
        return services;
    }
}