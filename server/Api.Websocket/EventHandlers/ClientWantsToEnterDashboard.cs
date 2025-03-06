using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models.Entities;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.EventHandlers;

public class ClientWantsToEnterDashboardDto : BaseDto
{
}

public class ServerAddsAdminToDashboard : BaseDto
{
    public List<Devicelog> Devicelogs { get; set; }
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