using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Web;
using Api.Websocket.EventHandlers.ServerEventDtos;
using Api.Websocket.Interfaces;
using Application.Interfaces.Infrastructure.Websocket;
using Fleck;
using WebSocketBoilerplate;

namespace Api.Websocket;

public class FleckWebSocketServerHost(WebApplication app, ILogger<FleckWebSocketServerHost> logger)
    : IWebSocketServerHost
{
    private WebSocketServer? _server;

    public Task StartAsync(int port)
    {
        Environment.SetEnvironmentVariable("PORT", port.ToString());
        var url = $"ws://0.0.0.0:{port}";
        logger.LogInformation("WS running on url: " + url);
        _server = new WebSocketServer(url);
        Action<IWebSocketConnection> config = ws =>
        {
            var queryString = ws.ConnectionInfo.Path.Split('?').Length > 1
                ? ws.ConnectionInfo.Path.Split('?')[1]
                : "";

            var id = HttpUtility.ParseQueryString(queryString)["id"] ?? throw new Exception("Please specify ID query param for websocket connection");
            using var scope = app.Services.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IConnectionManager>();

            ws.OnOpen = () => manager.OnOpen(ws, id); 
            ws.OnClose = () => manager.OnClose(ws, id);
            ws.OnError = ex =>
            {
                var problemDetails = new ServerSendsErrorMessage
                {
                    Error = ex.Message
                };
                ws.Send(JsonSerializer.Serialize(problemDetails));
            };
            ws.OnMessage = async message => 
            {
                try
                {
                    await app.CallEventHandler(ws, message);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error in handling message: {message}", message);
                    var baseDto = JsonSerializer.Deserialize<BaseDto>(message, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new BaseDto
                    {
                        eventType = nameof(ServerSendsErrorMessage)
                    };
                    var resp = new ServerSendsErrorMessage
                        { Message = e.Message, requestId = baseDto.requestId ?? "" };
                    ws.SendDto(resp);
                }
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

    // private int GetAvailablePort(int startPort)
    // {
    //     var port = startPort;
    //     var isPortAvailable = false;
    //
    //     do
    //     {
    //         try
    //         {
    //             var tcpListener = new TcpListener(IPAddress.Loopback, port);
    //             tcpListener.Start();
    //             tcpListener.Stop();
    //             isPortAvailable = true;
    //         }
    //         catch (SocketException)
    //         {
    //             port++;
    //         }
    //     } while (!isPortAvailable);
    //
    //     return port;
    // }
}