using Application.Interfaces.Infrastructure.Broadcasting;
using Fleck;
using lib;

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
        var server = new WebSocketServer("ws://0.0.0.0:8181");
        server.Start(ws =>
        {
            var connectionCreator = app.Services.GetRequiredService<IConnectionCreator>();
            var registry = app.Services.GetRequiredService<IConnectionRegistry>();
            var connection = connectionCreator.Create(ws);
            ws.OnOpen = () => registry.RegisterConnection(connection);
            ws.OnClose = () => registry.UnregisterConnection(connection);
            ws.OnError = ex => { Console.WriteLine(ex.Message); };
            ws.OnMessage = async message =>
            {
                try
                {
                    await app.InvokeClientEventHandler(ws, message, ServiceLifetime.Scoped);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException);
                    Console.WriteLine(ex.StackTrace);
                }
            };
        });


        return app;
    }
}