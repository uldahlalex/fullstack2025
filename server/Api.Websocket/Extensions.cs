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


    public static async Task<WebApplication> ConfigureWebsocketApi(this WebApplication app, int? productionPort = null)
    {
        app.UseRouting();
        var wsHost = new FleckWebSocketServerHost(app);
    
        // Get the actual port being used
        var serverAddress = app.Urls.Select(url => new Uri(url)).First();
        var port = serverAddress.Port;
    
        await wsHost.StartAsync(port);
    
        app.Lifetime.ApplicationStopping.Register(() => wsHost.Dispose());
        return app;
    }
}