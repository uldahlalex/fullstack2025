using Application.Interfaces.Infrastructure.Mqtt;
using Application.Models;
using Application.Models.Dtos;
using Core.Domain;
using Core.Domain.Entities;
using Infrastructure.Mqtt.PublishingHandlers;
using Infrastructure.Mqtt.SubscriptionHandlers;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Formatter;

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

    public static async Task<WebApplication> ConfigureMqtt(this WebApplication app, int mqttPort)
    {
        var appOptions = app.Services.GetRequiredService<IOptionsMonitor<AppOptions>>().CurrentValue;
        var mqttClient = app.Services.GetRequiredService<IMqttClient>();
        var eventBus = app.Services.GetRequiredService<MqttEventBus>();
        
        // Connect to MQTT broker
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(appOptions.MQTT_BROKER_HOST, 8883)
            .WithCredentials(appOptions.MQTT_USERNAME, appOptions.MQTT_PASSWORD)
            .WithClientId($"cloudrun_{Environment.GetEnvironmentVariable("K_SERVICE")}_{Guid.NewGuid()}")
            .WithTlsOptions(new MqttClientTlsOptions
            {
                UseTls = true,
                AllowUntrustedCertificates = true,
                IgnoreCertificateChainErrors = true,
                IgnoreCertificateRevocationErrors = true,
            })
            .WithCleanSession()
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
            .WithTimeout(TimeSpan.FromSeconds(30))
            .WithProtocolVersion(MqttProtocolVersion.V311) 
            .Build();

        using var scope = app.Services.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IMqttEventHandler>();
        
        await eventBus.RegisterHandlersAsync(handlers);

        return app;
    }
}