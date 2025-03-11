using System.ComponentModel.DataAnnotations;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Core.Domain.Entities;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.EventHandlers;

public class ClientWantsToEnterDashboardDto : BaseDto
{
}

public class ServerAddsAdminToDashboard : BaseDto
{
    [Required] public List<Devicelog> Devicelogs { get; set; } = null!;
}

public class ClientWantsToEnterDashboard(IConnectionManager connectionManager, IDataRepository repo)
    : BaseEventHandler<ClientWantsToEnterDashboardDto>
{
    public override async Task Handle(ClientWantsToEnterDashboardDto dto, IWebSocketConnection socket)
    {
        var clientId = await connectionManager.GetClientIdFromSocketId(socket.ConnectionInfo.Id.ToString());
        await connectionManager.AddToTopic("dashboard", clientId);
        var allMetrics = repo.GetAllMetrics();
        socket.SendDto(new ServerAddsAdminToDashboard
        {
            Devicelogs = allMetrics,
            requestId = dto.requestId
        });
    }
}