using WebSocketBoilerplate;

namespace Api.Websocket;

public static class Extensions
{
    public static IServiceCollection RegisterWebsocketApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        var assembly = typeof(Extensions).Assembly;
        services.InjectEventHandlers(assembly);
        return services;
    }


    public static WebApplication ConfigureWebsocketApi(this WebApplication app)
    {
        app.UseRouting();
        app.StartWsServer();
        return app;
    }
}