using System.Text.Json;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Models.Dtos;
using MQTTnet;

namespace Infrastructure.Mqtt.PublishingHandlers;

public class ChangePreferencesForDeviceHandler(IMqttClient mqttClient)
    : IMqttPublisher<AdminWantsToChangePreferencesForDeviceDto>
{
    public async Task Publish(AdminWantsToChangePreferencesForDeviceDto dto)
    {
        var serialized = JsonSerializer.Serialize(dto, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        string topic = $"device/{dto.DeviceId}/{nameof(AdminWantsToChangePreferencesForDeviceDto)}";
        
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(serialized)
            .Build();

        await mqttClient.PublishAsync(message);
    }
}