using Application.Interfaces.Infrastructure.Mqtt;
using MQTTnet;

namespace Infrastructure.Mqtt;

public static class Extensions
{
    public static IServiceCollection RegisterMqttInfrastructure(this IServiceCollection services)
    {
        // services.AddScoped<IMessagePublisher, >() todo
        services.AddSingleton<IMqttClientService, MqttClientService>();

        return services;
    }
    
    public static WebApplication ConfigureMqtt(this WebApplication app)
    {
       var mqttClientConnection = app.Services.GetRequiredService<IMqttClientService>();
       mqttClientConnection.ConnectAsync().Wait();
       mqttClientConnection.SubscribeAsync("messages").Wait();
        return app;
    }

}