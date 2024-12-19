public class MqttApi : IHostedService
{
    private readonly IMqttClientConnection _connection;
    private readonly ILogger<MqttApi> _logger;

    public MqttApi(IMqttClientConnection connection, ILogger<MqttApi> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _connection.OnMessageReceived += HandleIncomingMessage;
            await _connection.ConnectAsync(cancellationToken);
            await SubscribeToTopics();
            _logger.LogInformation("MQTT connection started");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start MQTT client");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task SubscribeToTopics()
    {
        if (!_connection.IsConnected)
            throw new InvalidOperationException("Client not connected");

        await _connection.SubscribeAsync("test");
    }

    private async Task HandleIncomingMessage(MqttMessage message)
    {
        try
        {
            _logger.LogInformation("Received message: {Message} on topic: {Topic}", 
                message.Payload, message.Topic);
            // todo Process message to SubscriptionHandlers
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message");
        }
    }
}