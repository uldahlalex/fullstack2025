using AsyncApi.Net.Generator.Attributes;
using Fleck;
using lib;
using realtimeapi;

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
public class ClientWantsToEcho(State state) : BaseEventHandler<ClientWantsToEchoDto>
{
    public override Task Handle(ClientWantsToEchoDto dto, IWebSocketConnection socket)
    {
        socket.SendDto(new ServerSendsEchoDto() { message = dto.message, client = socket.ConnectionInfo.Id});
        return Task.CompletedTask;
    }
}