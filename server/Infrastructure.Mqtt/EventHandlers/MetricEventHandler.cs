using System.Text.Json;
using Api;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Infrastructure.Mqtt.EventHandlers.Dtos;
using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt.EventHandlers;

public class MetricEventHandler(
    ILogger<MetricEventHandler> logger,
    IServiceLogic service,
    IDataRepository repo,
    IConnectionManager connectionManager)
    : IMqttEventHandler<MetricEventDto>
{
    public async Task HandleAsync(MetricEventDto eventDtoData)
    {
        // repo.AddMetric(eventDtoData);
        logger.LogInformation(JsonSerializer.Serialize(eventDtoData));
        await connectionManager.AddToTopic("admin",  "asd");
        await connectionManager.BroadcastToTopic("admin",JsonSerializer.Serialize(
                new { eventType = "temperature",
                   eventDtoData = eventDtoData }));
        
    }
}