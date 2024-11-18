using Fleck;
using lib;
using realtimeapi;

public class ClientWantsToEchoDto : BaseDto
{
    public string Message { get; set; }
}

public class ClientWantsToEcho(State state) : BaseEventHandler<ClientWantsToEchoDto>
{
    public override Task Handle(ClientWantsToEchoDto dto, IWebSocketConnection socket)
    {
        socket.Send(dto.Message);
        return Task.CompletedTask;
    }
}

