using Api.Websocket.EventHandlers.ClientEventDtos;
using Api.Websocket.EventHandlers.ServerEventDtos;
using NUnit.Framework;
using Startup.Tests.TestUtils;
using WebSocketBoilerplate;

namespace Startup.Tests.EventTests;

public class ClientWantsToEchoTest() : ApiTestBase(new ApiTestBaseConfig())
{
    [Test]
    public async Task ClientCanConnectAndRequestResponse()
    {
        _ = CreateClient();
        var wsPort = Environment.GetEnvironmentVariable("WS_PORT");

        if (string.IsNullOrEmpty(wsPort)) throw new Exception("Environment variable WS_PORT is not set");

        var url = "ws://localhost:" + wsPort ;

        var client = new WsRequestClient(
            new[] { typeof(ClientWantsToEchoDto).Assembly },
            url
        );

        await client.ConnectAsync();

        var dto = new ClientWantsToEchoDto
        {
            Message = "hey there"
        };

        var response = await client
            .SendMessage<ClientWantsToEchoDto, ServerSendsEchoDto>(dto);
        if(dto.Message !=response.Message)
            throw new Exception("Expected response to be the same as the request");
    }

    [Test]
    public async Task ClientCanConnectAndRequestResponse2()
    {
        _ = CreateClient();
        var wsPort = Environment.GetEnvironmentVariable("WS_PORT");

        if (string.IsNullOrEmpty(wsPort)) throw new Exception("Environment variable WS_PORT is not set");

        var url = "ws://localhost:" + wsPort;

        var client = new WsRequestClient(
            new[] { typeof(ClientWantsToEchoDto).Assembly },
            url
        );

        await client.ConnectAsync();

        var dto = new ClientWantsToEchoDto
        {
            Message = "hey there"
        };

        var response = await client
            .SendMessage<ClientWantsToEchoDto, ServerSendsEchoDto>(dto);
        if(dto.Message!= response.Message)
            throw new Exception("Expected response to be the same as the request");
    }
}