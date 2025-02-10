using System.Reflection;
using System.Text.Json;
using Api.Websocket.Events;
using Websocket.Client;
using Fix;

using Xunit.Abstractions;

namespace Startup.Tests.EventTests;

public class
    ClientWantsToEchoTest(ITestOutputHelper outputHelper)// : ApiTestBase(outputHelper, new ApiTestBaseConfig() {MockProxyConfig = true})
{

    
    [Fact]
    public async Task ClientCanConnectAndRequestResponse()
    {
        outputHelper.WriteLine($"Test starting. ServerSendsEchoDto is in assembly: {typeof(ServerSendsEchoDto).Assembly.FullName}");
    
        var client = new WsRequestClient(
            new[] { 
                typeof(ClientWantsToEchoDto).Assembly,
                typeof(ServerSendsEchoDto).Assembly
            },
            "ws://localhost:8181", 
            outputHelper
        );
    
        await client.ConnectAsync();
    
        var dto = new ClientWantsToEchoDto()
        {
            Message = "hey there",
            eventType = "ClientWantsToEcho"  // Make sure this is set
        };
    
        outputHelper.WriteLine("Sending request...");
        var response = await client.SendMessage<ClientWantsToEchoDto, ServerSendsEchoDto>(dto);
        Assert.Equal(dto.Message, response.Message);
    }
}