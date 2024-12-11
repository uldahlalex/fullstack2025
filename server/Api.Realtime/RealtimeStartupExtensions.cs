using Api.Realtime.Events;
using AsyncApi.Net.Generator;
using AsyncApi.Net.Generator.AsyncApiSchema.v2;
using Fleck;
using lib;

namespace Api.Realtime;

public static class RealtimeStartupExtensions
{
    public static HashSet<Type> Services { get; set; } = new();

    public static WebApplicationBuilder AddDependenciesForRealtimeApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddAsyncApiSchemaGeneration(o =>
        {
            o.AssemblyMarkerTypes = new[] { typeof(ClientWantsToEcho) };
            o.Middleware.UiTitle = "API Documentation";
            o.AsyncApi = new AsyncApiDocument
            {
                Info = new Info { Title = "My application", Description = "Fullstack 2025" }
            };
        });
        builder.Services.AddSingleton<State>();
        var assembly = typeof(State).Assembly;
        Services = builder.FindAndInjectClientEventHandlers(assembly, ServiceLifetime.Scoped);

        return builder;
    }


    public static WebApplication AddMiddlewareForRealtimeApi(this WebApplication app)
    {
        app.MapAsyncApiDocuments();
        app.MapAsyncApiUi();
        app.UseRouting();
        var server = new WebSocketServer("ws://0.0.0.0:8181");
        server.Start(ws =>
        {
            ws.OnOpen = () => { app.Services.GetRequiredService<State>().Connections.TryAdd(Guid.NewGuid(), ws); };
            ws.OnClose = () =>
            {
                var state = app.Services.GetRequiredService<State>();
                var key = state.Connections.FirstOrDefault(x => x.Value == ws).Key;
                state.Connections.TryRemove(key, out _);
            };
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