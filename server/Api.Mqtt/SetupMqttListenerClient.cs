// API Layer - Handles subscriptions and incoming messages

using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;

public class MqttListener : IHostedService
{
    private readonly IMqttClient _mqttClient;
    private readonly IMessageProcessor _processor;
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .WithProtocolVersion(MqttProtocolVersion.V500)
            .Build();
        await _mqttClient.ConnectAsync(mqttClientOptions);
        await SubscribeToTopics();
        
        _mqttClient.ApplicationMessageReceivedAsync += HandleIncomingMessage;

    }

    private async Task SubscribeToTopics()
    {
        var topicFilter = new MqttTopicFilterBuilder()
            .WithTopic("test")
            .Build();
        await _mqttClient.SubscribeAsync(topicFilter);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task HandleIncomingMessage(MqttApplicationMessageReceivedEventArgs args)
    {
        var incomingText = args.ApplicationMessage.ConvertPayloadToString();
        var message = new Message()
        {
            MessageString = incomingText
        };
        await _processor.ProcessAsync(message);
    }
}