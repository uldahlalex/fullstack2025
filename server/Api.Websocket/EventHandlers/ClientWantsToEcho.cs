using System.Text.Json;
using Api.Websocket.EventHandlers.ClientEventDtos;
using Api.Websocket.EventHandlers.ServerEventDtos;
using Application.Interfaces;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.EventHandlers;

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