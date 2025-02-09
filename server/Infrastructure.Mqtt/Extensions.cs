using Application.Interfaces.Infrastructure.Mqtt;
using Infrastructure.Mqtt.Events;
using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt;

public static class Extensions
{
    public static IServiceCollection RegisterMqttInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IEventDispatcher, EventDispatcher>();

        services.AddScoped<IMqttEventHandler<TemperatureEvent>, TemperatureEventHandler>();
        services.AddScoped<IMqttEventHandler<HumidityEvent>, HumidityEventHandler>();

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
            EventDispatcher.TopicMappings.Keys.ToList().ForEach(async void (topicStrings) =>
            {
                await mqttService.SubscribeAsync(topicStrings);
            });
        });

        return app;
    }
}