using System.Text.Json;
using Application.Interfaces;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Events;

public class ClientWantsToEchoDto : BaseDto
{
    public string Message { get; set; }
}

public class ServerSendsEchoDto : BaseDto
{
    public string Message { get; set; }
}

public class ClientWantsToEcho(IServiceLogic service) : BaseEventHandler<ClientWantsToEchoDto>
{
    public override Task Handle(ClientWantsToEchoDto dto, IWebSocketConnection socket)
    {
        var message = new ServerSendsEchoDto { Message = dto.Message };
        service.Broadcast(message, "");
        return Task.CompletedTask;
    }
}