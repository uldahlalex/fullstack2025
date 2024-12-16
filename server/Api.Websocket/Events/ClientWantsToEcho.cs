using System.Text.Json;
using Application.Interfaces;
using AsyncApi.Net.Generator.Attributes;
using Fleck;
using lib;

namespace Api.Websocket.Events;

public class ClientWantsToEchoDto : BaseDto
{
    public string message { get; set; }
}

public class ServerSendsEchoDto : BaseDto
{
    public string message { get; set; }
    public Guid client { get; set; }
}

[SubscribeOperation<ClientWantsToEchoDto>(nameof(ClientWantsToEcho))]
[PublishOperation<ServerSendsEchoDto>(nameof(ServerSendsEchoDto))]
public class ClientWantsToEcho(IServiceLogic service) : BaseEventHandler<ClientWantsToEchoDto>
{
    public override Task Handle(ClientWantsToEchoDto dto, IWebSocketConnection socket)
    {
        var message = JsonSerializer.Serialize(new ServerSendsEchoDto
            { message = JsonSerializer.Serialize(service.GetDomainModels()), client = socket.ConnectionInfo.Id });

        var info = socket.ConnectionInfo.Id;
        service.Broadcast(message, socket.ConnectionInfo.Id);
        return Task.CompletedTask;
    }
}