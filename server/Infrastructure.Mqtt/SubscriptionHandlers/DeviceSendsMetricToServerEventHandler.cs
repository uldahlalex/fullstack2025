using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Core.Domain.Entities;
using HiveMQtt.Client.Events;
using HiveMQtt.MQTT5.Types;

namespace Infrastructure.Mqtt.SubscriptionHandlers;

public class DeviceSendsMetricToServerEventHandler(
    ILogger<DeviceSendsMetricToServerEventHandler> logger,
    IDataRepository repo,
    IConnectionManager connectionManager
    ) : IMqttMessageHandler
{
    public string TopicFilter => "device/+/DeviceSendsMetricToServerDto";
    public QualityOfService QoS => QualityOfService.ExactlyOnceDelivery;

    public void Handle(object? sender, OnMessageReceivedEventArgs onMessageReceivedEventArgs)
    {
        var deserialized =
            JsonSerializer.Deserialize<DeviceSendsMetricToServerDto>(onMessageReceivedEventArgs.PublishMessage
                .PayloadAsString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ??
            throw new Exception("Could not deserialize " + onMessageReceivedEventArgs.PublishMessage.PayloadAsString +
                                " as an " + nameof(DeviceSendsMetricToServerDto));

        logger.LogInformation(JsonSerializer.Serialize(deserialized));
        repo.AddMetric(new Devicelog()
        {
        
        });
        connectionManager.BroadcastToTopic("alex",
            new ServerSendsMetricToAdminDto() { Metrics= repo.GetAllMetrics() });
        

    }
}