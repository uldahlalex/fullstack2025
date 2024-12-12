using Api.Realtime.Events;
using AsyncApi.Net.Generator;
using AsyncApi.Net.Generator.AsyncApiSchema.v2;
using Fleck;
using lib;
using service;
using service.Interfaces;

namespace Api.Realtime;

public static class WebSocketExtensions
{
    public static HashSet<Type> Services { get; set; } = new();

    public static WebApplicationBuilder AddDependenciesForRealtimeApi(this WebApplicationBuilder builder)
    {
             var assembly = typeof(WebSocketExtensions).Assembly;
        Services = builder.FindAndInjectClientEventHandlers(assembly, ServiceLifetime.Scoped);
        return builder;
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
                    await app.InvokeClientEventHandler(Services, ws, message, ServiceLifetime.Scoped);
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