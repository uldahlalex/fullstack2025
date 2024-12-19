using System.Text.Json;
using Application.Interfaces;
using AsyncApi.Net.Generator.Attributes;
using Fleck;
using lib;

namespace Api.Websocket.Events;

public class ClientWantsToEchoDto : BaseDto
{
    public string Message { get; set; }
}

public class ServerSendsEchoDto : BaseDto
{
    public string Message { get; set; }
}

[SubscribeOperation<ClientWantsToEchoDto>(nameof(ClientWantsToEcho))]
[PublishOperation<ServerSendsEchoDto>(nameof(ServerSendsEchoDto))]
public class ClientWantsToEcho(IServiceLogic service) : BaseEventHandler<ClientWantsToEchoDto>
{
    public override Task Handle(ClientWantsToEchoDto dto, IWebSocketConnection socket)
    {
        var message = new ServerSendsEchoDto { Message = dto.Message };
        service.Broadcast(message, socket.ConnectionInfo.Id);
        return Task.CompletedTask;
    }
}