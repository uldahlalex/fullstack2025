using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Core.Domain;
using Fleck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WebSocketBoilerplate;

namespace Infrastructure.Websocket;

public static class Extensions
{
    public static IServiceCollection AddWebsocketInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionManager, WebSocketConnectionManager>();
        return services;
    }
}