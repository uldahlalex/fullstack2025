using System.Text;
using System.Text.Json;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Models;
using HiveMQtt.Client;
using HiveMQtt.Client.Exceptions;
using HiveMQtt.MQTT5.Types;
using Infrastructure.Mqtt;
using Infrastructure.Mqtt.PublishingHandlers;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Formatter;

public static class MqttExtensions
{

    
public static IServiceCollection RegisterMqttInfrastructure(this IServiceCollection services)
{
    services.AddSingleton<HiveMQClient>(sp =>
{
    var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<AppOptions>>();
    var logger = sp.GetRequiredService<ILogger<HiveMQClient>>();
    
    var options = new HiveMQClientOptionsBuilder()
        .WithBroker(optionsMonitor.CurrentValue.MQTT_BROKER_HOST)
        .WithPort(8883)
        .WithClientId($"myClientId_{Guid.NewGuid()}") 
        .WithUseTls(true)
        .WithAllowInvalidBrokerCertificates(true) 
        .WithCleanStart(true)
        .WithKeepAlive(30) 
        .WithAutomaticReconnect(true) 
        .WithMaximumPacketSize(1024)
        .WithReceiveMaximum(100)
        .WithSessionExpiryInterval(3600)
        .WithUserName(optionsMonitor.CurrentValue.MQTT_USERNAME)
        .WithPassword(optionsMonitor.CurrentValue.MQTT_PASSWORD)
        .WithRequestProblemInformation(true)
        .WithRequestResponseInformation(true)
        .Build();

    var client = new HiveMQClient(options);

    client.OnDisconnectReceived += (sender, args) =>
    {
        logger.LogWarning("MQTT client disconnected");
    };

    client.OnMessageReceived += (eventObject, args) =>
    {
        logger.LogInformation("Global handler - Message Received: {sender}", 
            JsonSerializer.Serialize(eventObject));
        logger.LogInformation("Global handler - Message Received: {payload}", 
            args.PublishMessage.PayloadAsString);
    };

    const int maxRetries = 5;
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            logger.LogInformation("Attempting to connect to MQTT broker (attempt {attempt}/{maxRetries})", 
                attempt, maxRetries);

            var connectResult = client.ConnectAsync().GetAwaiter().GetResult();
            
            logger.LogInformation("Connection successful on attempt {attempt}. Result: {result}", 
                attempt, 
                JsonSerializer.Serialize(new
                {
                    connectResult.ResponseInformation,
                    connectResult.ReasonString
                }));
                
            break; // Connection successful
        }
        catch (HiveMQttClientException ex)
        {
            logger.LogError(ex, "Error connecting to MQTT broker on attempt {attempt}", attempt);
            
            if (attempt == maxRetries)
                throw;
                
            // Exponential backoff
            Thread.Sleep(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
        }
    }

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

public static async Task<WebApplication> SetupMqttClient(this WebApplication app)
{
    var _options = app.Services.GetRequiredService<IOptionsMonitor<AppOptions>>().CurrentValue;
    var _logger = app.Services.GetRequiredService<ILogger<string>>();
    var mqttFactory = new MqttClientFactory();
    var client = mqttFactory.CreateMqttClient();

    var options = new MqttClientOptionsBuilder()
        .WithTcpServer(_options.MQTT_BROKER_HOST, 8883)
        .WithCredentials(_options.MQTT_USERNAME, _options.MQTT_PASSWORD)
        .WithClientId($"cloudrun_{Environment.GetEnvironmentVariable("K_SERVICE")}_{Guid.NewGuid()}")
        .WithTlsOptions(new MqttClientTlsOptions
        {
            UseTls = true,
            AllowUntrustedCertificates = true,
            IgnoreCertificateChainErrors = true,
            IgnoreCertificateRevocationErrors = true,
            CertificateValidationHandler = _ => true 
        })
        .WithCleanSession()
        .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
        .WithTimeout(TimeSpan.FromSeconds(30))
        .WithProtocolVersion(MqttProtocolVersion.V311) 
        .Build();

    var connectTimeout = TimeSpan.FromSeconds(30);
    var connectCancellation = new CancellationTokenSource(connectTimeout);

    try 
    {
        var connection = await client.ConnectAsync(options, connectCancellation.Token);
        _logger.LogInformation(connection.ResponseInformation);
        _logger.LogInformation(connection.ReasonString);
    }
    catch (OperationCanceledException)
    {
        _logger.LogError("Connection attempt timed out after {Timeout} seconds", connectTimeout.TotalSeconds);
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to connect to MQTT broker");
        throw;
    }

    await client.SubscribeAsync("/helloworld");
    //log when message
    return app;
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
