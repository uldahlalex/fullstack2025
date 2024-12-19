using Application.Interfaces.Api.Mqtt;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Packets;

namespace Api.Mqtt;

public interface IMqttApi
{
    Task StartAsync(CancellationToken cancellationToken);
    Task SubscribeToTopics();
    Task StopAsync(CancellationToken cancellationToken);
    Task HandleIncomingMessage(MqttApplicationMessageReceivedEventArgs args);
}

/// <summary>
/// Mqtt API could also have been a websocket implementation subscribing to MQTT
/// </summary>
/// <param name="logger"></param>
public class MqttApi(ILogger<MqttApi> logger) : IMqttApi, IHostedService
{
    private readonly IMqttClient _mqttClient = new MqttFactory().CreateMqttClient();


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _mqttClient.ApplicationMessageReceivedAsync += HandleIncomingMessage;

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883)
                .WithProtocolVersion(MqttProtocolVersion.V500)
                .WithClientId("MyClientId_" + Guid.NewGuid()) 
                .WithCleanSession() 
                .Build();

            var response = await _mqttClient.ConnectAsync(mqttClientOptions, cancellationToken);

            if (response.ResultCode != MqttClientConnectResultCode.Success)
                throw new Exception($"Failed to connect: {response.ResultCode}");

            await SubscribeToTopics();
            Console.WriteLine("MQTT connection started");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to start MQTT client: {ex}");
            throw;
        }
    }

    public async Task SubscribeToTopics()
    {
        try
        {
            if (!_mqttClient.IsConnected) throw new Exception("Client not connected");

            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic("test")
                .Build();

            var result = await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptions
            {
                TopicFilters = new List<MqttTopicFilter> { topicFilter }
            });

            Console.WriteLine($"Subscribed to topic: {topicFilter.Topic}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to subscribe: {ex}");
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_mqttClient.IsConnected) await _mqttClient.DisconnectAsync();
    }

    public async Task HandleIncomingMessage(MqttApplicationMessageReceivedEventArgs args)
    {
        try
        {
            var incomingText = args.ApplicationMessage.ConvertPayloadToString();
            var message = new Message
            {
                MessageString = incomingText
            };
            logger.LogInformation($"Received message: {incomingText}");

            // var applicationMessage = new MqttApplicationMessageBuilder()
            //     .WithTopic("test2")
            //     .WithPayload("Hello from the server")
            //     .Build();

            // await _mqttClient.PublishAsync(applicationMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling message: {ex}");
        }
    }
}