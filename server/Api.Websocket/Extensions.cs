using Api.Websocket.Documentation;
using WebSocketBoilerplate;

namespace Api.Websocket;

public static class Extensions
{
    public static IServiceCollection RegisterWebsocketApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument(config =>
        {
            config.DocumentName = "ws";
            config.Version = "v1";
            config.DocumentProcessors.Add(new AddAllDerivedTypesProcessor());
            config.DocumentProcessors.Add(new AddStringConstantsProcessor());
    
        });       
        var assembly = typeof(Extensions).Assembly;
        services.InjectEventHandlers(assembly);
        return services;
    }


    public static WebApplication ConfigureWebsocketApi(this WebApplication app)
    {
        app.UseOpenApi(config =>
        {
            //set path
            app.UseOpenApi(options =>
            {
                options.Path = "/openapi/ws.json";
            });
        });
        app.UseRouting();
        app.StartWsServer();
        return app;
    }
}