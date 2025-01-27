using Application.Interfaces.Infrastructure.Mqtt;
using MQTTnet;

namespace Infrastructure.Mqtt;

public static class Extensions
{
    public static IServiceCollection RegisterMqttInfrastructure(this IServiceCollection services)
    {
        // services.AddScoped<IMessagePublisher, >() todo
        services.AddSingleton<IMqttClientConnection, MqttConnectionProvider>();

        return services;
    }

}