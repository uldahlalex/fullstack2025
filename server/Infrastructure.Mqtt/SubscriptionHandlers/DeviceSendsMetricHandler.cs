// Infrastructure/Mqtt/Handlers/DeviceMetricsHandler.cs

using System.Text.Json;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models.Entities;

namespace Infrastructure.Mqtt.SubscriptionHandlers;

public class DeviceMetricsHandler : IMqttEventHandler
{
    private readonly IDataRepository _dataRepository;
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<DeviceMetricsHandler> _logger;

    public string TopicPattern => "device/+/DeviceSendsMetricToServerDto";

    public DeviceMetricsHandler(
        IDataRepository dataRepository, 
        IConnectionManager connectionManager,
        ILogger<DeviceMetricsHandler> logger)
    {
        _dataRepository = dataRepository;
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task HandleAsync(MqttEvent evt)
    {
        _logger.LogInformation($"Received event: {evt.Topic} {evt.Payload} {JsonSerializer.Serialize(evt.Parameters)}");
        
        var metric = JsonSerializer.Deserialize<DeviceSendsMetricToServerDto>(
            evt.Payload, 
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
        )?.ToDeviceLog() ?? throw new Exception("Could not parse as " + nameof(Devicelog));
        
        _dataRepository.AddMetric(metric);
        var allLogs = _dataRepository.GetAllMetrics();
        
        var broadcast = new ServerSendsMetricToAdmin()
        {
            Metrics = allLogs,
            eventType = nameof(ServerSendsMetricToAdmin)
        };
        
        await _connectionManager.BroadcastToTopic("dashboard", broadcast);
    }
}