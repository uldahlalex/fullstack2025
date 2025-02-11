using System.Net;
using System.Net.Sockets;
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

public class FleckWebSocketServerHost(WebApplication app, ILogger<FleckWebSocketServerHost> logger)
    : IWebSocketServerHost
{
    private readonly WebApplication _app = app;
    private readonly ILogger<FleckWebSocketServerHost> _logger = logger;
    private WebSocketServer? _server;

    public Task StartAsync(int port)
    {
        port = GetAvailablePort(port);

        // Set the dynamic port to an environment variable
        Environment.SetEnvironmentVariable("WS_PORT", port.ToString());

        var url = $"ws://0.0.0.0:{port}/ws";
        logger.LogInformation("WS running on url: "+url);
        _server = new WebSocketServer(url);

        
        Action<IWebSocketConnection> config = ws =>
        {
            using var scope = app.Services.CreateScope();
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
                        await app.CallEventHandler(ws, message);
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
    private int GetAvailablePort(int startPort)
    {
        int port = startPort;
        bool isPortAvailable = false;

        do
        {
            try
            {
                TcpListener tcpListener = new TcpListener(IPAddress.Loopback, port);
                tcpListener.Start();
                tcpListener.Stop();
                isPortAvailable = true;
            }
            catch (SocketException)
            {
                port++;
            }
        } while (!isPortAvailable);

        return port;
    }
}
public class ServerSendsErrorMessage : BaseDto
{
    public string Error { get; set; }
    public string RequestId { get; set; }
}