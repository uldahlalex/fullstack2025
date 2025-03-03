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
       
        var dto = new ClientWantsToEchoDto
        {
            Message = "hey there"
        };

        var response = await _wsClient
            .SendMessage<ClientWantsToEchoDto, ServerSendsEchoDto>(dto);
        if(dto.Message !=response.Message)
            throw new Exception("Expected response to be the same as the request");
    }

    [Test]
    public async Task ClientCanConnectAndRequestResponse2()
    {
      
        
        var dto = new ClientWantsToEchoDto
        {
            Message = "hey there"
        };

        var response = await _wsClient
            .SendMessage<ClientWantsToEchoDto, ServerSendsEchoDto>(dto);
        if(dto.Message!= response.Message)
            throw new Exception("Expected response to be the same as the request");
    }
}