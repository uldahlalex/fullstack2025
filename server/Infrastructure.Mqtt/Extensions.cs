using Application.Interfaces.Infrastructure.Mqtt;
using Infrastructure.Mqtt.EventHandlers;
using Infrastructure.Mqtt.EventHandlers.Dtos;
using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt;

public static class Extensions
{
    public static IServiceCollection RegisterMqttInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IMqttEventHandler<MetricEventDto>, MetricEventHandler>();
        services.AddSingleton<IMqttClientService, MqttClientService>();
        return services;
    }

    public static WebApplication ConfigureMqtt(this WebApplication app)
    {
        _ = Task.Run(async () =>
        {
            using var scope = app.Services.CreateScope();
            var mqttService = scope.ServiceProvider.GetRequiredService<IMqttClientService>();
            await mqttService.ConnectAsync();
            foreach (var topic in mqttService.GetSubscriptionTopics()) await mqttService.SubscribeAsync(topic);
        });

        return app;
    }
}