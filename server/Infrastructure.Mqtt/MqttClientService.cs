using Application.Interfaces.Infrastructure.Mqtt;
using Application.Models;
using Infrastructure.Mqtt.Interfaces;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Protocol;

namespace Infrastructure.Mqtt;

public class MqttClientService : IMqttClientService, IDisposable
{
    private readonly IMqttClient _client;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ILogger<MqttClientService> _logger;
    private readonly MqttClientOptions _options;
    private readonly HashSet<string> _topicSubscriptions;
    private bool _isDisposed;

    public MqttClientService(
        IOptionsMonitor<AppOptions> optionsMonitor,
        ILogger<MqttClientService> logger,
        IEventDispatcher eventDispatcher)
    {
        _logger = logger;
        _eventDispatcher = eventDispatcher;
        _topicSubscriptions = new HashSet<string>();

        var tlsOptions = new MqttClientTlsOptions
        {
            UseTls = true
        };
        _client = new MqttClientFactory().CreateMqttClient();
        _options = new MqttClientOptionsBuilder()
            .WithTcpServer(optionsMonitor.CurrentValue.MQTT_BROKER_HOST, 8883)
            .WithCredentials(optionsMonitor.CurrentValue.MQTT_USERNAME, optionsMonitor.CurrentValue.MQTT_PASSWORD)
            .WithTlsOptions(tlsOptions)
            .WithCleanSession()
            .Build();

        _client.DisconnectedAsync += HandleDisconnection;
        _client.ApplicationMessageReceivedAsync += HandleMessage;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task PublishAsync(string topic, string payload, bool retain = false, int qos = 1)
    {
        try
        {
            if (!IsConnected) await ConnectAsync();

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qos)
                .WithRetainFlag(retain)
                .Build();

            var result = await _client.PublishAsync(message);

            if (result.ReasonCode != MqttClientPublishReasonCode.Success)
                throw new Exception($"Failed to publish message. Result: {result.ReasonCode}");

            _logger.LogDebug("Published message to topic {Topic}: {Payload}", topic, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to topic {Topic}", topic);
            throw;
        }
    }

    public bool IsConnected => _client?.IsConnected ?? false;

    public async Task ConnectAsync()
    {
        try
        {
            if (IsConnected) return;

            var result = await _client.ConnectAsync(_options);
            if (result.ResultCode != MqttClientConnectResultCode.Success)
                throw new Exception($"Failed to connect to MQTT broker. Result: {result.ResultCode}");

            _logger.LogInformation("Successfully connected to MQTT broker");

            // Resubscribe to all topics if we have any
            await ResubscribeToTopics();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to MQTT broker");
            throw;
        }
    }

    public async Task DisconnectAsync()
    {
        try
        {
            if (_client.IsConnected)
            {
                var disconnectOptions = new MqttClientDisconnectOptionsBuilder().Build();
                await _client.DisconnectAsync(disconnectOptions);
                _logger.LogInformation("Disconnected from MQTT broker");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disconnecting from MQTT broker");
            throw;
        }
    }

    public async Task SubscribeAsync(string topic)
    {
        try
        {
            if (!IsConnected) throw new InvalidOperationException("Cannot subscribe: MQTT client is not connected");

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topic, MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            var result = await _client.SubscribeAsync(subscribeOptions);

            var firstResult = result.Items.FirstOrDefault();
            if (firstResult?.ResultCode != MqttClientSubscribeResultCode.GrantedQoS1 &&
                firstResult?.ResultCode != MqttClientSubscribeResultCode.GrantedQoS0)
                throw new Exception($"Failed to subscribe to topic {topic}. Result: {firstResult?.ResultCode}");

            _topicSubscriptions.Add(topic);
            _logger.LogInformation("Successfully subscribed to topic: {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to topic: {Topic}", topic);
            throw;
        }
    }

    public async Task UnsubscribeAsync(string topic)
    {
        try
        {
            if (!IsConnected) throw new InvalidOperationException("Cannot unsubscribe: MQTT client is not connected");

            var unsubscribeOptions = new MqttClientUnsubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();

            var result = await _client.UnsubscribeAsync(unsubscribeOptions);

            var firstResult = result.Items.FirstOrDefault();
            if (firstResult?.ResultCode != MqttClientUnsubscribeResultCode.Success)
                throw new Exception($"Failed to unsubscribe from topic {topic}. Result: {firstResult?.ResultCode}");

            _topicSubscriptions.Remove(topic);
            _logger.LogInformation("Successfully unsubscribed from topic: {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unsubscribe from topic: {Topic}", topic);
            throw;
        }
    }

    public IReadOnlyCollection<string> GetSubscribedTopics()
    {
        return _topicSubscriptions.ToList().AsReadOnly();
    }

    private async Task HandleDisconnection(MqttClientDisconnectedEventArgs args)
    {
        _logger.LogWarning("Disconnected from MQTT broker: {Reason}", args.Reason);

        try
        {
            // Wait 5 seconds before reconnecting
            await Task.Delay(TimeSpan.FromSeconds(5));
            await ConnectAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reconnect to MQTT broker");
        }
    }

    private async Task HandleMessage(MqttApplicationMessageReceivedEventArgs args)
    {
        try
        {
            var topic = args.ApplicationMessage.Topic;
            var payload = args.ApplicationMessage.ConvertPayloadToString();

            _logger.LogDebug("Received message on topic {Topic}: {Payload}", topic, payload);

            await _eventDispatcher.DispatchAsync(topic, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling MQTT message on topic: {Topic}",
                args.ApplicationMessage.Topic);
        }
    }

    private async Task ResubscribeToTopics()
    {
        if (_topicSubscriptions.Count == 0) return;

        try
        {
            var subscribeOptions = new MqttClientSubscribeOptionsBuilder();
            foreach (var topic in _topicSubscriptions)
                subscribeOptions.WithTopicFilter(
                    topic,
                    MqttQualityOfServiceLevel.AtLeastOnce
                );

            await _client.SubscribeAsync(subscribeOptions.Build());
            _logger.LogInformation("Resubscribed to {Count} topics", _topicSubscriptions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resubscribe to topics");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            if (_client.IsConnected)
                _client.DisconnectAsync().GetAwaiter().GetResult();
            _client.Dispose();
        }

        _isDisposed = true;
    }
}