using Api.Websocket.Events;
using WebSocketBoilerplate;
using Xunit.Abstractions;

namespace Startup.Tests.EventTests;

public class
    ClientWantsToEchoTest(ITestOutputHelper outputHelper) : ApiTestBase(outputHelper)
{

    
    [Fact]
    public async Task ClientCanConnectAndRequestResponse()
    {
        var httpClient = CreateClient();
        var wsUri = new UriBuilder(httpClient.BaseAddress!)
        {
            Scheme = "ws",
            Path = "/" 
        }.Uri;

        outputHelper.WriteLine($"Connecting to WebSocket at: {wsUri}");
    
        var client = new WsRequestClient(
            new[] { typeof(ClientWantsToEchoDto).Assembly },
            wsUri.ToString()
        );
    
        await client.ConnectAsync();
    
        var dto = new ClientWantsToEchoDto()
        {
            Message = "hey there",
        };
    
        var response = await client.SendMessage<ClientWantsToEchoDto, ServerSendsEchoDto>(dto);
        Assert.Equal(dto.Message, response.Message);
    }
}