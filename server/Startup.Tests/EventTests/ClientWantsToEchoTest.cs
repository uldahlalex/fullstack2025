using Api.Websocket.Events;
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
        _ = CreateClient();
        string wsPort = Environment.GetEnvironmentVariable("WS_PORT");

        if (string.IsNullOrEmpty(wsPort))
        {
            throw new Exception("Environment variable WS_PORT is not set");
        }
        
        string url = "ws://localhost:" + wsPort;
        _outputHelper.WriteLine($"Connecting to WebSocket at: {url}");

        var client = new WsRequestClient(
            new[] { typeof(ClientWantsToEchoDto).Assembly },
            url
        );

        await client.ConnectAsync();
        _outputHelper.WriteLine("Successfully connected to WebSocket");

        var dto = new ClientWantsToEchoDto
        {
            Message = "hey there",
        };

        var response = await client
            .SendMessage<ClientWantsToEchoDto, ServerSendsEchoDto>(dto);
        Assert.Equal(dto.Message, response.Message);
    }
    
    [Fact]
    public async Task ClientCanConnectAndRequestResponse2()
    {
        _ = CreateClient();
        string wsPort = Environment.GetEnvironmentVariable("WS_PORT");

        if (string.IsNullOrEmpty(wsPort))
        {
            throw new Exception("Environment variable WS_PORT is not set");
        }
        
        string url = "ws://localhost:" + wsPort;
        _outputHelper.WriteLine($"Connecting to WebSocket at: {url}");

        var client = new WsRequestClient(
            new[] { typeof(ClientWantsToEchoDto).Assembly },
            url
        );

        await client.ConnectAsync();
        _outputHelper.WriteLine("Successfully connected to WebSocket");

        var dto = new ClientWantsToEchoDto
        {
            Message = "hey there",
        };

        var response = await client
            .SendMessage<ClientWantsToEchoDto, ServerSendsEchoDto>(dto);
        Assert.Equal(dto.Message, response.Message);
    }
}