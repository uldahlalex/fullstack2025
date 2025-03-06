using System.Text.Json;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
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
    : IMqttEventHandler<IoTDeviceSendsMetricEventDto>
{
    public async Task HandleAsync(IoTDeviceSendsMetricEventDto eventDtoData)
    {
        var deviceLog = new Devicelog
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