using Api.Realtime.Events;
using AsyncApi.Net.Generator;
using AsyncApi.Net.Generator.AsyncApiSchema.v2;
using Fleck;
using lib;
using service.Interfaces;

namespace Api.Realtime;

public static class RealtimeStartupExtensions
{
    public static HashSet<Type> Services { get; set; } = new();

    public static WebApplicationBuilder AddDependenciesForRealtimeApi(this WebApplicationBuilder builder)
    {
        // builder.Services.AddAsyncApiSchemaGeneration(o =>
        // {
        //     o.AssemblyMarkerTypes = new[] { typeof(ClientWantsToEcho) };
        //     o.Middleware.UiTitle = "API Documentation";
        //     o.AsyncApi = new AsyncApiDocument
        //     {
        //         Info = new Info { Title = "My application", Description = "Fullstack 2025" }
        //     };
        // });
        var assembly = typeof(RealtimeStartupExtensions).Assembly;
        Services = builder.FindAndInjectClientEventHandlers(assembly, ServiceLifetime.Scoped);
        return builder;
    }


    public static WebApplication AddMiddlewareForRealtimeApi(this WebApplication app)
    {
        // app.MapAsyncApiDocuments();
        // app.MapAsyncApiUi();
        app.UseRouting();
        var server = new WebSocketServer("ws://0.0.0.0:8181");
        server.Start(ws =>
        {
            server.Start(ws =>
            {
                var adapter = new IWebSocketConnectionAdapter(ws);
                var registry = app.Services.GetRequiredService<IConnectionRegistry>();

                ws.OnOpen = () => registry.RegisterConnection(adapter);
                ws.OnClose = () => registry.UnregisterConnection(adapter);
            });
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