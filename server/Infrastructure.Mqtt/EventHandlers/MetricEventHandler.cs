using System.Text.Json;
using Api;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models.Dtos;
using Application.Models.Entities;
using Infrastructure.Mqtt.EventHandlers.Dtos;
using Infrastructure.Mqtt.Interfaces;

namespace Infrastructure.Mqtt.EventHandlers;

public class ServerSendsMetricToAdmin : ApplicationBaseDto
{
    public List<Devicelog> Metrics { get; set; }
    public string eventType { get; set; }
    
}

public class MetricEventHandler(
    ILogger<MetricEventHandler> logger,
    IServiceLogic service,
    IDataRepository repo,
    IConnectionManager connectionManager)
    : IMqttEventHandler<MetricEventDto>
{
    public async Task HandleAsync(MetricEventDto eventDtoData)
    {
        logger.LogInformation(JsonSerializer.Serialize(eventDtoData));
        var deviceLog = new Devicelog()
        {
            Timestamp = DateTime.UtcNow,
            Deviceid = eventDtoData.DeviceId,
            Unit = eventDtoData.Unit,
            Value = eventDtoData.Value,
            Id = Guid.NewGuid().ToString()
        };
        repo.AddMetric(deviceLog);
        var serverSendsMetricToAdmin = new ServerSendsMetricToAdmin
        {
            Metrics = repo.GetAllMetrics(),
            eventType = nameof(ServerSendsMetricToAdmin)
        };
        await connectionManager.BroadcastToTopic("dashboard", serverSendsMetricToAdmin);
        
    }
}