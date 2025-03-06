using System.Text.Json;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Models;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Infrastructure.Mqtt;

public static class Extensions
{
    public static IServiceCollection RegisterMqttInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMqttClient>(new MqttClientFactory().CreateMqttClient());
        services.AddSingleton<MqttEventBus>();
        return services;
    }

    public static async Task<WebApplication> ConfigureMqtt(this WebApplication app)
    {
        var appOptions = app.Services.GetRequiredService<IOptionsMonitor<AppOptions>>().CurrentValue;
        var mqttClient = app.Services.GetRequiredService<IMqttClient>();
        var eventBus =  app.Services.GetRequiredService<MqttEventBus>();
        await mqttClient.ConnectAsync(new MqttClientOptionsBuilder()
            .WithTcpServer(appOptions.MQTT_BROKER_HOST, 1883)
            .WithCredentials(appOptions.MQTT_USERNAME, appOptions.MQTT_PASSWORD)
            .WithTlsOptions(new MqttClientTlsOptions
            {
                UseTls = true
            })
            .Build());

        await eventBus.SubscribeAsync("device/+/status", async (evt) =>
        {
            var payload = JsonSerializer.Serialize(new {});
            var message = new MqttApplicationMessageBuilder()
                .WithTopic($"device/{""}/command")
                .WithPayload(payload)
                .Build();

            await mqttClient.PublishAsync(message);
        });

        await eventBus.SubscribeAsync("device/+/telemetry", (evt) =>
        {
            // Handle telemetry
        });

        return app;
    }
}