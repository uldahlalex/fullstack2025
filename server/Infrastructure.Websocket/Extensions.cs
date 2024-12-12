using Microsoft.Extensions.DependencyInjection;
using service.Interfaces;

namespace Infrastructure.Websocket;

public static class Extensions
{
    public static IServiceCollection AddWebsocketInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IState, State>();
        services.AddScoped<IBroadcaster, Broadcaster>();
        return services;
    }
}