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
    public Guid Client { get; set; }
}

[SubscribeOperation<ClientWantsToEchoDto>(nameof(ClientWantsToEcho))]
[PublishOperation<ServerSendsEchoDto>(nameof(ServerSendsEchoDto))]
public class ClientWantsToEcho(IServiceLogic service) : BaseEventHandler<ClientWantsToEchoDto>
{
    public override Task Handle(ClientWantsToEchoDto dto, IWebSocketConnection socket)
    {
        var message = JsonSerializer.Serialize(new ServerSendsEchoDto
            { Message = JsonSerializer.Serialize(service.GetDomainModels()), Client = socket.ConnectionInfo.Id });

        var info = socket.ConnectionInfo.Id;
        service.Broadcast(message, socket.ConnectionInfo.Id);
        return Task.CompletedTask;
    }
}