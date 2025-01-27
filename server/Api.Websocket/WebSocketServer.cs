using System.Text.Json;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Websocket;
using Fleck;
using Microsoft.AspNetCore.Mvc;
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
            var connection = scopedServices.GetRequiredService<IConnectionCreator>().Create(ws);
            ws.OnOpen = () => scopedServices.GetRequiredService<IConnectionRegistry>().RegisterConnection(connection);
            ws.OnClose = () =>
                scopedServices.GetRequiredService<IConnectionRegistry>().UnregisterConnection(connection);
            ws.OnError = ex =>
            {
                var problemDetails = new ProblemDetails
                {
                    Title = ex.Message,
                    Detail = ex.InnerException?.Message
                };
                scopedServices.GetRequiredService<IServiceLogic>()
                    .Broadcast(JsonSerializer.Serialize(problemDetails), ws.ConnectionInfo.Id);
            };
            ws.OnMessage = async message =>
            {
                try
                {
                    await app.CallEventHandler(ws, message);
                }
                catch (Exception ex)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Title = ex.Message,
                        Detail = ex.InnerException?.Message
                    };
                    scopedServices.GetRequiredService<IServiceLogic>()
                        .Broadcast(JsonSerializer.Serialize(problemDetails), ws.ConnectionInfo.Id);
                }
            };
        };
        server.Start(config);
        return app;
    }
}