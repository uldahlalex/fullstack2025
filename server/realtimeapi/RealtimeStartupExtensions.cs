
using System.Text.Json;
using AsyncApi.Net.Generator;
using AsyncApi.Net.Generator.AsyncApiSchema.v2;
using Fleck;
using lib;


namespace realtimeapi;

public static class RealtimeStartupExtensions
{
    
    public static WebApplicationBuilder AddDependenciesForRealtimeApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<State>();
        var assembly = typeof(ClientWantsToEcho).Assembly;
        Services = builder.FindAndInjectClientEventHandlers(assembly, ServiceLifetime.Scoped);

        return builder;
    }

    public static HashSet<Type> Services { get; set; } = new();

    public static WebApplication AddMiddlewareForRealtimeApi(this WebApplication app)
    {
        app.UseRouting();
         // app.MapHub<ChatHub>("/chatHub");
      //   app.MapAsyncApiDocuments();
        // app.MapAsyncApiUi();
        Console.WriteLine(JsonSerializer.Serialize(Services.FirstOrDefault().Name));
        var server = new WebSocketServer("ws://0.0.0.0:8181");
        server.Start(ws =>
        {
           
            ws.OnOpen = () =>
            {
                app.Services.GetRequiredService<State>().Connections.TryAdd(Guid.NewGuid(), ws);
            };
            ws.OnClose = () =>
            {
                var state = app.Services.GetRequiredService<State>();
                var key = state.Connections.FirstOrDefault(x => x.Value == ws).Key;
                state.Connections.TryRemove(key, out _);
            };
            ws.OnError = ex =>
            {
                Console.WriteLine(ex.Message);
            };
            ws.OnMessage = async message =>
            {
                try
                {
                    await app.InvokeClientEventHandler(Services, ws, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }; 
        });
        
        
         
        return app;
    }
}