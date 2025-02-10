using Api.Websocket;
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

    public static async Task<WebApplication> ConfigureWebsocketApi(this WebApplication app)
    {
        app.UseRouting();

        var factory = app.Services.GetRequiredService<IWebSocketServerHostFactory>();
        var wsHost = factory.Create(app);
    
        // Get the actual port being used
        // var serverAddress = app.Urls.Select(url => new Uri(url)).First();
        // var port = serverAddress.Port;
    
        await wsHost.StartAsync(5000);
    
        app.Lifetime.ApplicationStopping.Register(() => wsHost.Dispose());
        return app;
    }
}

public interface IWebSocketServerHostFactory
{
    IWebSocketServerHost Create(WebApplication app);
}

public class FleckWebSocketServerHostFactory : IWebSocketServerHostFactory
{
    private readonly ILogger<FleckWebSocketServerHost> _logger;

    public FleckWebSocketServerHostFactory(ILogger<FleckWebSocketServerHost> logger)
    {
        _logger = logger;
    }

    public IWebSocketServerHost Create(WebApplication app)
    {
        return new FleckWebSocketServerHost(app, _logger);
    }
}