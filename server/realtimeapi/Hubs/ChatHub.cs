using AsyncApi.Net.Generator.Attributes;
using Microsoft.AspNetCore.SignalR;

namespace realtimeapi.Hubs;


public class ChatHub : Hub
{

    [PublishOperation<MySecondPayloadMessageType>("my_queue_name")]
    public async Task SendMessage(string user, string message)
    {
        Console.WriteLine($"Received message from {user}: {message}");
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
    
    public async Task Subscribe()
    {
        Console.WriteLine("Subscribing to messages");
        await Groups.AddToGroupAsync(Context.ConnectionId, "messages");
    }
}

public class MySecondPayloadMessageType
{
    public string Name { get; set; }
    public string Description { get; set; }
}
