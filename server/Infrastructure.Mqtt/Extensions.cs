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
        // Create a background task to handle MQTT setup
        _ = Task.Run(async () =>
        {
            using var scope = app.Services.CreateScope();
            var mqttService = scope.ServiceProvider.GetRequiredService<IMqttClientService>();

            await mqttService.ConnectAsync();

            await mqttService.SubscribeAsync("sensors/+/temperature");
            await mqttService.SubscribeAsync("sensors/+/humidity");
        });

        return app;
    }
}