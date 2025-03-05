using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket.EventHandlers;

public class ClientWantsToEnterDashboardDto : BaseDto
{
    
}

public  class ClientWantsToEnterDashboard(IConnectionManager connectionManager) : BaseEventHandler<ClientWantsToEnterDashboardDto>
{
    public override async Task Handle(ClientWantsToEnterDashboardDto dto, IWebSocketConnection socket)
    {
        var clientId = await connectionManager.GetClientIdFromSocketId(socket.ConnectionInfo.Id.ToString());
        await connectionManager.AddToTopic("dashboard", clientId );
        
    }
}