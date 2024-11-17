using AsyncApi.Net.Generator.Attributes;
using Microsoft.AspNetCore.SignalR;

namespace realtimeapi.Hubs;

public class ChatHub : Hub
{

    [PublishOperation<MyPayloadMessageType>("my_queue_name")]
    [PublishOperation<MyPayloadMessageType, MySecondPayloadMessageType>("my_queue_second_name")]

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
    
    //subscribe to messages signalR
    public async Task Subscribe()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "messages");
    }
}

public class MySecondPayloadMessageType
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class MyPayloadMessageType
{
    public string Name { get; set; }
    public string Description { get; set; }
}
