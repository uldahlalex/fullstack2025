using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using MQTTnet.Packets;

public interface IMqttListener
{
    Task StartAsync(CancellationToken cancellationToken);
    Task SubscribeToTopics();
    Task StopAsync(CancellationToken cancellationToken);
    Task HandleIncomingMessage(MqttApplicationMessageReceivedEventArgs args);
}

public class MqttListener : IMqttListener, IHostedService
{
    private readonly IMqttClient _mqttClient;
    // private readonly IMessageProcessor _processor;
    
    public MqttListener(
        // IMessageProcessor processor
        )
    {
        // Create the client in constructor
        _mqttClient = new MqttFactory().CreateMqttClient();
        // _processor = processor;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try 
        {
            // Set up the handler before connecting
            _mqttClient.ApplicationMessageReceivedAsync += HandleIncomingMessage;

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883)
                .WithProtocolVersion(MqttProtocolVersion.V500)
                .WithClientId("MyClientId_" + Guid.NewGuid()) // Add unique client ID
                .WithCleanSession() // Add this for clean session
                .Build();

            var response = await _mqttClient.ConnectAsync(mqttClientOptions, cancellationToken);
            
            if (response.ResultCode != MqttClientConnectResultCode.Success)
            {
                throw new Exception($"Failed to connect: {response.ResultCode}");
            }

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
            if (!_mqttClient.IsConnected)
            {
                throw new Exception("Client not connected");
            }

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
        if (_mqttClient.IsConnected)
        {
            await _mqttClient.DisconnectAsync();
        }
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
            Console.WriteLine($"Received message: {incomingText}");

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic("test2")
                .WithPayload("Hello from the server")
                .Build();

            await _mqttClient.PublishAsync(applicationMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling message: {ex}");
        }
    }
}

