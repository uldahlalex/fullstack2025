using System.Text.Json;
using Application.Interfaces;
using Application.Services;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.Events;

public class ClientWantsToEchoDto : BaseDto
{
    public string Jwt { get; set; }
    public string Message { get; set; }
}

public class ServerSendsEchoDto : BaseDto
{
    public string Message { get; set; }
}

public class ClientWantsToEcho(IServiceLogic service, ISecurityService securityService) : BaseEventHandler<ClientWantsToEchoDto>
{
    public override Task Handle(ClientWantsToEchoDto dto, IWebSocketConnection socket)
    {
        var claims = securityService.VerifyJwtOrThrow(dto.Jwt);
        var message = new ServerSendsEchoDto { Message = dto.Message };
        service.Broadcast(message, "");
        service.GetDomainModels(claims);
        return Task.CompletedTask;
    }
}