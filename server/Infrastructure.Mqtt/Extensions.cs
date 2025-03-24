using System.Text.Json;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Models;
using HiveMQtt.Client;
using HiveMQtt.MQTT5.Types;
using Infrastructure.Mqtt;
using Infrastructure.Mqtt.PublishingHandlers;
using Microsoft.Extensions.Options;

public static class MqttExtensions
{
    
public static IServiceCollection RegisterMqttInfrastructure(this IServiceCollection services)
{
    services.AddSingleton<HiveMQClient>(sp =>
    {
        var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<AppOptions>>();
        var logger = sp.GetRequiredService<ILogger<HiveMQClient>>();
        
        var client = new HiveMQClient(new HiveMQClientOptionsBuilder()
            .WithBroker(optionsMonitor.CurrentValue.MQTT_BROKER_HOST)
            .WithUserName(optionsMonitor.CurrentValue.MQTT_USERNAME)
            .WithPassword(optionsMonitor.CurrentValue.MQTT_PASSWORD)
            .WithPort(8883)
            .WithUseTls(true)
            .Build());

        client.OnMessageReceived += (eventObject, args) =>
        {
            logger.LogInformation("Global handler - Message Received: {sender}", 
                JsonSerializer.Serialize(eventObject));
            logger.LogInformation("Global handler - Message Received: {payload}", 
                args.PublishMessage.PayloadAsString);
        };

        var connectResult = client.ConnectAsync().GetAwaiter().GetResult();
        logger.LogInformation("Connection result: {result}", JsonSerializer.Serialize(new
        {
            connectResult.ResponseInformation,
            connectResult.ReasonString
        }));

        return client;
    });

    //here we're adding the handlers to the DI
    var subscribeHandlers = typeof(IMqttMessageHandler).Assembly
        .GetTypes()
        .Where(t => !t.IsAbstract && typeof(IMqttMessageHandler).IsAssignableFrom(t));
    foreach (var handlerType in subscribeHandlers)
    {
        services.AddScoped(handlerType);
    }

    services.AddSingleton<IMqttPublisher, MqttPublisher>();
    return services;
}

public static async Task<WebApplication> ConfigureMqtt(this WebApplication app)
{
    var mqttClient = app.Services.GetRequiredService<HiveMQClient>();
    var builder = new SubscribeOptionsBuilder();
    var logger = app.Services.GetRequiredService<ILogger<HiveMQClient>>();

    var handlerTypes = typeof(IMqttMessageHandler).Assembly
        .GetTypes()
        .Where(t => !t.IsAbstract && typeof(IMqttMessageHandler).IsAssignableFrom(t));

    //here we're subscribing to each handler
    foreach (var handlerType in handlerTypes)
    {
        using (var scope = app.Services.CreateScope())
        {
            var handler = (IMqttMessageHandler)scope.ServiceProvider
                .GetRequiredService(handlerType);

            logger.LogInformation("Subscribing to topic: {topic} with QoS: {qos}", 
                handler.TopicFilter, handler.QoS);

            builder.WithSubscription(
                new TopicFilter(handler.TopicFilter, handler.QoS),
                (sender, args) =>
                {
                    using var messageScope = app.Services.CreateScope();
                    var messageHandler = (IMqttMessageHandler)messageScope.ServiceProvider
                        .GetRequiredService(handlerType);
                    messageHandler.Handle(sender, args);
                });
        }
    }

    await mqttClient.SubscribeAsync(builder.Build());
    return app;
}
}
