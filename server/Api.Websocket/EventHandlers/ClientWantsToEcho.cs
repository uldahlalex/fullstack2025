using System.Net;
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
    IConnectionManager manager,
    ILogger<ClientWantsToEcho> logger)
    : BaseEventHandler<ClientWantsToEchoDto>
{
    public override async Task Handle(ClientWantsToEchoDto dto, IWebSocketConnection socket)
    {
        var clientId = await manager.GetClientIdFromSocketId(socket.ConnectionInfo.Id.ToString());
        logger.LogInformation(socket.ConnectionInfo.Id + " has sent: " + JsonSerializer.Serialize(dto));
        var join = manager.AddToTopic("messages", clientId);
        var message = new ServerSendsEchoDto { Message = dto.Message, requestId = dto.requestId };
        Console.WriteLine(JsonSerializer.Serialize(message));
        await manager.BroadcastToTopic("messages", message);
    }
}