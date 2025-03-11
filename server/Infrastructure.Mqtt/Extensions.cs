using Application.Interfaces.Infrastructure.Mqtt;
using Application.Models;
using Application.Models.Dtos;
using Core.Domain;
using Core.Domain.Entities;
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
        
        services.AddSingleton<IMqttPublisher<AdminWantsToChangePreferencesForDeviceDto>, 
            ChangePreferencesForDeviceHandler>();
        
        services.AddScoped<DeviceMetricsHandler>();
        services.AddScoped<IMqttEventHandler, DeviceMetricsHandler>();
        
        return services;
    }

    public static async Task<WebApplication> ConfigureMqtt(this WebApplication app)
    {
        var appOptions = app.Services.GetRequiredService<IOptionsMonitor<AppOptions>>().CurrentValue;
        var mqttClient = app.Services.GetRequiredService<IMqttClient>();
        var eventBus = app.Services.GetRequiredService<MqttEventBus>();
        
        // Connect to MQTT broker
        await mqttClient.ConnectAsync(new MqttClientOptionsBuilder()
            .WithTcpServer(appOptions.MQTT_BROKER_HOST, 8883)
            .WithCredentials(appOptions.MQTT_USERNAME, appOptions.MQTT_PASSWORD)
            .WithTlsOptions(new MqttClientTlsOptions
            {
                UseTls = true
            })
            .Build());

        // Get all handlers from the service provider
        using var scope = app.Services.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IMqttEventHandler>();
        
        // Set up subscriptions
        await eventBus.RegisterHandlersAsync(handlers);

        return app;
    }
}