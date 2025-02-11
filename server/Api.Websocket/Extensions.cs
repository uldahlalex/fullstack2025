using WebSocketBoilerplate;

public static class Extensions
{
    public static IServiceCollection RegisterWebsocketApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        var assembly = typeof(Extensions).Assembly;
        services.InjectEventHandlers(assembly);
        services.AddSingleton<IWebSocketServerHostFactory, FleckWebSocketServerHostFactory>();
        return services;
    }

    public static async Task<WebApplication> ConfigureWebsocketApi(this WebApplication app, int wsPort = 8181)
    {
        app.UseRouting();

        var factory = app.Services.GetRequiredService<IWebSocketServerHostFactory>();
        var wsHost = factory.Create(app);


        await wsHost.StartAsync(wsPort);

        app.Lifetime.ApplicationStopping.Register(() => wsHost.Dispose());
        return app;
    }
}