using System.Text.Json;
using Application.Interfaces.Infrastructure.Websocket;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket;

public static class CustomWebSocketServer
{
    public static WebApplication StartWsServer(this WebApplication app)
    {
        var scopedServices = app.Services.CreateScope().ServiceProvider;

        var server = new WebSocketServer("ws://0.0.0.0:8181");
        Action<IWebSocketConnection> config = ws =>
        {
            ws.OnOpen = () =>
                scopedServices.GetRequiredService<IWebSocketService<IWebSocketConnection>>().RegisterConnection(ws);
            ws.OnClose = () => { };
            //  scopedServices.GetRequiredService<IWebSocketService<IWebSocketConnection>>().OnClose(ws);
            ws.OnError = ex =>
            {
                var problemDetails = new ServerSendsErrorMessage
                {
                    Error = ex.Message
                };
                ws.Send(JsonSerializer.Serialize(problemDetails));
            };
            ws.OnMessage = message =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await app.CallEventHandler(ws, message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.InnerException);
                        Console.WriteLine(e.StackTrace);
                        var baseDto = JsonSerializer.Deserialize<BaseDto>(message);
                        ws.SendDto(new ServerSendsErrorMessage { Error = e.Message, RequestId = baseDto.requestId });
                    }
                });
            };
        };
        server.Start(config);
        return app;
    }
}

public class ServerSendsErrorMessage : BaseDto
{
    public string Error { get; set; }
    public string RequestId { get; set; }
}