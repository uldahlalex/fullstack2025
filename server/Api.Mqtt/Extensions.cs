using System.Net.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Mqtt;

public static class Extensions
{
    public static IServiceCollection AddDependenciesForMqttApi(this IServiceCollection services)
    {
        services.AddHostedService<MqttListener>(); // Change to AddHostedService
        return services;

    }
    
    public static WebApplication AddMiddlewareForMqttApi(this WebApplication app)
    {
        // var mqttListener = app.Services.GetRequiredService<IMqttListener>();
        // mqttListener.StartAsync(CancellationToken.None);
        return app;
    }

}