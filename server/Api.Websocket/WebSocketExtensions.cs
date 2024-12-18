using System.Text.Json;
using Application;
using Application.Interfaces.Infrastructure.Broadcasting;
using Fleck;
using lib;
using Microsoft.AspNetCore.Mvc;

namespace Api.Websocket;

public static class WebSocketExtensions
{
    public static IServiceCollection AddDependenciesForRealtimeApi(this IServiceCollection services)
    {
        var assembly = typeof(WebSocketExtensions).Assembly;
        services.FindAndInjectClientEventHandlers(assembly, ServiceLifetime.Scoped);
        return services;
    }


    public static WebApplication AddMiddlewareForRealtimeApi(this WebApplication app)
    {
        app.UseRouting();
        app.StartWsServer();
        return app;
    }
}