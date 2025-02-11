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

public class ClientWantsToEcho(
    IServiceLogic service,
    ISecurityService securityService,
    ILogger<ClientWantsToEcho> logger)
    : BaseEventHandler<ClientWantsToEchoDto>
{
    public override Task Handle(ClientWantsToEchoDto dto, IWebSocketConnection socket)
    {
        logger.LogInformation(socket.ConnectionInfo.Id + " has sent: " + JsonSerializer.Serialize(dto));
        var message = new ServerSendsEchoDto { Message = dto.Message, requestId = dto.requestId };
        socket.SendDto(message);
        return Task.CompletedTask;
    }
}