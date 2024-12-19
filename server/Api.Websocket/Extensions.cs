using lib;

namespace Api.Websocket;

public static class Extensions
{
    public static IServiceCollection RegisterWebsocketApiServices(this IServiceCollection services)
    {
        var assembly = typeof(Extensions).Assembly;
        services.FindAndInjectClientEventHandlers(assembly, ServiceLifetime.Scoped);
        return services;
    }


    public static WebApplication ConfigureWebsocketApi(this WebApplication app)
    {
        app.UseRouting();
        app.StartWsServer();
        return app;
    }
}