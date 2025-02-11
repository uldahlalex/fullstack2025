using Api.Websocket.Events;
using Startup;
using Startup.Tests;
using WebSocketBoilerplate;

public class ClientWantsToEchoTest : ApiTestBase
{
    private readonly ITestOutputHelper _outputHelper;

    public ClientWantsToEchoTest(ITestOutputHelper outputHelper) 
        : base(outputHelper, new ApiTestBaseConfig())
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task ClientCanConnectAndRequestResponse()
    {
     
        
        var wsUri = new UriBuilder
        {
            Scheme = "ws",
            Host = "localhost",
            Path = "/ws"
        }.Uri.ToString();

        _outputHelper.WriteLine($"Connecting to WebSocket at: {wsUri}");

        var client = new WsRequestClient(
            new[] { typeof(ClientWantsToEchoDto).Assembly },
            wsUri
        );

  
            await client.ConnectAsync().WaitAsync(TimeSpan.FromSeconds(5));
            _outputHelper.WriteLine("Successfully connected to WebSocket");

            var dto = new ClientWantsToEchoDto
            {
                Message = "hey there",
            };

            var response = await client
                .SendMessage<ClientWantsToEchoDto, ServerSendsEchoDto>(dto)
                .WaitAsync(TimeSpan.FromSeconds(5));
                
            Assert.Equal(dto.Message, response.Message);

    }
}