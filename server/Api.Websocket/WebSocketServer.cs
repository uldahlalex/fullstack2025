using System.Text.Json;
using Application.Interfaces.Infrastructure.Websocket;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket;

public interface IWebSocketServerHost : IDisposable
{
    Task StartAsync(int port);
    Task StopAsync();
}

public class FleckWebSocketServerHost : IWebSocketServerHost
{
    private readonly WebApplication _app;
    private WebSocketServer? _server;

    public FleckWebSocketServerHost(WebApplication app)
    {
        _app = app;
    }

    public Task StartAsync(int port)
    {
        _server = new WebSocketServer($"ws://0.0.0.0:{port}");
        
        Action<IWebSocketConnection> config = ws =>
        {
            using var scope = _app.Services.CreateScope();
            var wsService = scope.ServiceProvider.GetRequiredService<IWebSocketService<IWebSocketConnection>>();
            
            ws.OnOpen = () => wsService.RegisterConnection(ws);
            ws.OnClose = () => { };
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
                        await _app.CallEventHandler(ws, message);
                    }
                    catch (Exception e)
                    {
                        var baseDto = JsonSerializer.Deserialize<BaseDto>(message);
                        ws.SendDto(new ServerSendsErrorMessage { Error = e.Message, RequestId = baseDto.requestId });
                    }
                });
            };
        };

        _server.Start(config);
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _server?.Dispose();
        _server = null;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _server?.Dispose();
    }
}
public class ServerSendsErrorMessage : BaseDto
{
    public string Error { get; set; }
    public string RequestId { get; set; }
}