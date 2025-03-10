using System.Text.Json;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Application.Models.Dtos;
using Application.Models.Entities;
using Infrastructure.Mqtt.PublishingHandlers;
using Infrastructure.Mqtt.SubscriptionHandlers;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Infrastructure.Mqtt;

public static class Extensions
{
    public static IServiceCollection RegisterMqttInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMqttClient>(new MqttClientFactory().CreateMqttClient());
        services.AddSingleton<MqttEventBus>();
        services.AddSingleton<IMqttPublisher<AdminWantsToChangePreferencesForDeviceDto>, ChangePreferencesForDeviceHandler>();
        services.AddSingleton<IEnumerable<IMqttEventHandler>>(sp => 
        {
            var handlerTypes = new[]
            {
                typeof(DeviceMetricsHandler),
            };
            
            return handlerTypes.Select(t => (IMqttEventHandler)sp.GetRequiredService(t)).ToList();
        });
        return services;
    }

    public static async Task<WebApplication> ConfigureMqtt(this WebApplication app)
    {
        var serviceProvider = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider;

        var appOptions = app.Services.GetRequiredService<IOptionsMonitor<AppOptions>>().CurrentValue;
        var mqttClient = app.Services.GetRequiredService<IMqttClient>();
        var eventBus =  app.Services.GetRequiredService<MqttEventBus>();
        await mqttClient.ConnectAsync(new MqttClientOptionsBuilder()
            .WithTcpServer(appOptions.MQTT_BROKER_HOST, 8883)
            .WithCredentials(appOptions.MQTT_USERNAME, appOptions.MQTT_PASSWORD)
            .WithTlsOptions(new MqttClientTlsOptions
            {
                UseTls = true
            })
            .Build());

        var handlers = app.Services.GetRequiredService<IEnumerable<IMqttEventHandler>>();
        await eventBus.SubscribeWithHandlerAsync(handlers);

        return app;
    }
}

public class ServerSendsMetricToAdmin : ApplicationBaseDto
{
    public List<Devicelog> Metrics { get; set; } = new List<Devicelog>();
}