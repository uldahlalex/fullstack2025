using WebSocketBoilerplate;

namespace Infrastructure.Websocket;

public class MemberLeftNotification : BaseDto
{
    public string ClientId { get; set; } = null!;
    public string Topic { get; set; } = null!;
}