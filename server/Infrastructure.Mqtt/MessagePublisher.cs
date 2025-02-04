using Application.Models;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.Formatter;
using MQTTnet.Implementations;
using MQTTnet.Protocol;

public class MqttClientService : IMqttClientService
{
    private readonly IMqttClient _client;
    private readonly ILogger<MqttClientService> _logger;
    private readonly MqttClientOptions _options;

    public event Func<MqttMessage, Task> OnMessageReceived;

    public MqttClientService(ILogger<MqttClientService> logger, IOptionsMonitor<AppOptions> optionsMonitor)
    {
        _client = new MqttClient(new MqttClientAdapterFactory(), new MqttNetEventLogger());
        _logger = logger;
        var tlsOptions = new MqttClientTlsOptions
        {
            UseTls = true,
            AllowUntrustedCertificates = true, // Only during development!
            IgnoreCertificateChainErrors = true, // Only during development!
            IgnoreCertificateRevocationErrors = true, // Only during development!
        };

        _options = new MqttClientOptionsBuilder()
            .WithTcpServer(optionsMonitor.CurrentValue.MQTT_BROKER_HOST, 8883)
            .WithCredentials(optionsMonitor.CurrentValue.MQTT_USERNAME, optionsMonitor.CurrentValue.MQTT_PASSWORD)
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .WithClientId($"MyClientId_{Guid.NewGuid()}")
            .WithCleanSession()
            .WithTlsOptions(tlsOptions)
            .Build();
        _client.ApplicationMessageReceivedAsync += HandleMessage;
    }

    public bool IsConnected => _client.IsConnected;

    public async Task ConnectAsync()
    {
        try
        {
            var response = await _client.ConnectAsync(_options);
            if (response.ResultCode != MqttClientConnectResultCode.Success)
            {
                throw new Exception($"Failed to connect: {response.ResultCode}");
            }
            _logger.LogInformation("Successfully connected to MQTT broker");
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
                var disconnectOptions = new MqttClientDisconnectOptions
                {
                    Reason = (MqttClientDisconnectOptionsReason)MqttClientDisconnectReason.NormalDisconnection,
                    ReasonString = "Normal disconnection"
                };
                await _client.DisconnectAsync(disconnectOptions);
                _logger.LogInformation("Successfully disconnected from MQTT broker");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during MQTT disconnect");
            throw;
        }
    }

    public async Task SubscribeAsync(string topic)
    {
        try
        {
            if (!_client.IsConnected)
            {
                throw new InvalidOperationException("Cannot subscribe: MQTT client is not connected");
            }

            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic(topic)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topicFilter)
                .Build();

            var result = await _client.SubscribeAsync(subscribeOptions);
            
            var firstResult = result.Items.FirstOrDefault();
            if (firstResult?.ResultCode != MqttClientSubscribeResultCode.GrantedQoS1 &&
                firstResult?.ResultCode != MqttClientSubscribeResultCode.GrantedQoS0 &&
                firstResult?.ResultCode != MqttClientSubscribeResultCode.GrantedQoS2)
            {
                throw new Exception($"Failed to subscribe to topic {topic}. Result: {firstResult?.ResultCode}");
            }

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
            if (!_client.IsConnected)
            {
                throw new InvalidOperationException("Cannot unsubscribe: MQTT client is not connected");
            }

            var unsubscribeOptions = new MqttClientUnsubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();

            var result = await _client.UnsubscribeAsync(unsubscribeOptions);
            
            var firstResult = result.Items.FirstOrDefault();
            if (firstResult?.ResultCode != MqttClientUnsubscribeResultCode.Success)
            {
                throw new Exception($"Failed to unsubscribe from topic {topic}. Result: {firstResult?.ResultCode}");
            }

            _logger.LogInformation("Successfully unsubscribed from topic: {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unsubscribe from topic: {Topic}", topic);
            throw;
        }
    }

    private async Task HandleMessage(MqttApplicationMessageReceivedEventArgs args)
    {
        try
        {
            if (OnMessageReceived != null)
            {
                var message = new MqttMessage
                {
                    Topic = args.ApplicationMessage.Topic,
                    Payload = args.ApplicationMessage.ConvertPayloadToString(),
                    Timestamp = DateTime.UtcNow
                };

                await OnMessageReceived(message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling MQTT message");
        }
    }
}