using System.Text.Json;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Application.Models.Dtos;
using Application.Models.Entities;
using Microsoft.Extensions.Options;
using MQTTnet;

namespace Infrastructure.Mqtt;

public static class Extensions
{
    public static IServiceCollection RegisterMqttInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMqttClient>(new MqttClientFactory().CreateMqttClient());
        services.AddSingleton<MqttEventBus>();
        services.AddSingleton<IMqttPublisher, MqttPublisher>();
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

        await eventBus.SubscribeAsync("device/+/metric", async (evt) =>
        {
            serviceProvider.GetRequiredService<ILogger<MqttEventBus>>().LogInformation($"Received event: {evt.Topic} {evt.Payload} {JsonSerializer.Serialize(evt.Parameters)}");
            var metric = JsonSerializer.Deserialize<DeviceSendsMetricToServerDto>(evt.Payload, new JsonSerializerOptions() {PropertyNameCaseInsensitive = true})?.ToDeviceLog() ?? throw new Exception("Could not parse as "+nameof(Devicelog));
            serviceProvider.GetRequiredService<IDataRepository>().AddMetric(metric);
            var allLogs = serviceProvider.GetRequiredService<IDataRepository>().GetAllMetrics();
            var broadcast = new ServerSendsMetricToAdmin()
            {
                Metrics = allLogs,
                eventType = nameof(ServerSendsMetricToAdmin)
            };
          
            await serviceProvider.GetRequiredService<IConnectionManager>().BroadcastToTopic("dashboard", broadcast);
        });

        await eventBus.SubscribeAsync("device/+/telemetry", async (evt) =>
        {
 
        });

        return app;
    }
}

public class MqttPublisher(IMqttClient mqttClient) : IMqttPublisher
{
    public async Task Publish(string topic, string payload)
    {
        var message = new MqttApplicationMessageBuilder()
            .WithTopic($"device/{""}/command")
            .WithPayload(payload)
            .Build();

        await mqttClient.PublishAsync(message);
    }
}

public class DeviceSendsMetricToServerDto
{
    public string Unit { get; set; }
    public string DeviceId { get; set; }
    public int Value { get; set; }

    public Devicelog ToDeviceLog()
    {
        var result = new Devicelog()
        {
Unit =  Unit,
Value = Value,
Id = Guid.NewGuid().ToString(),
Timestamp = DateTime.UtcNow,
Deviceid = DeviceId
        };
        return result;
    }
}

public class ServerSendsMetricToAdmin : ApplicationBaseDto
{
    public List<Devicelog> Metrics { get; set; } = new List<Devicelog>();
}