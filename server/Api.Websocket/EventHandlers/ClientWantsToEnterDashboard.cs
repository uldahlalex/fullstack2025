using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Core.Domain.Entities;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.EventHandlers;

public class ClientWantsToEnterDashboardDto : BaseDto
{
    public string Jwt { get; set; }   
}

public class ServerAddsAdminToDashboard : BaseDto
{
    [Required] public List<Devicelog> Devicelogs { get; set; } = null!;
}

public class ClientWantsToEnterDashboard(
    IConnectionManager connectionManager, 
    ISecurityService securityService,
    IDataRepository repo)
    : BaseEventHandler<ClientWantsToEnterDashboardDto>
{
    public override async Task Handle(ClientWantsToEnterDashboardDto dto, IWebSocketConnection socket)
    {
        securityService.VerifyJwtOrThrow(dto.Jwt);
        var clientId =  connectionManager.GetClientIdFromSocket(socket.ConnectionInfo.Id.ToString());
        await connectionManager.AddToTopic("dashboard", clientId);
        var allMetrics = repo.GetAllMetrics();
        socket.SendDto(new ServerAddsAdminToDashboard
        {
            Devicelogs = allMetrics,
            requestId = dto.requestId
        });
        socket.SendDto(new ServerConfirmsAdditionToDashboard());
    }
}

public class ServerConfirmsAdditionToDashboard : BaseDto
{
    public string Message { get; set; } = "You have been added to the dashboard";
}