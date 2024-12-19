using Application.Interfaces.Infrastructure.Mqtt;
using MQTTnet;
using MQTTnet.Client;

namespace Infrastructure.Mqtt;

public static class Extensions
{
    public static IServiceCollection RegisterMqttInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMqttClientConnection, MqttConnectionProvider>();

        return services;
    }

}