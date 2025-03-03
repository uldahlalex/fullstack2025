using System.Text.Json;
using Api;
using Api.Websocket.EventHandlers.ClientEventDtos;
using Api.WebSockets;
using Fleck;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Startup;
using WebSocketBoilerplate;
using Microsoft.Extensions.DependencyInjection;
using Startup.Tests.TestUtils;


namespace NUnit;

[TestFixture(typeof(DictionaryConnectionManager))]
public class ConnectionWithWsClient(Type connectionManagerType) : ApiTestBase
{

 


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
          
        });
    }


    [Theory]
    public async Task Api_Can_Successfully_Add_Connection()
    {
        var pairForClientId = _connectionManager.GetAllConnectionIdsWithSocketId().Result
            .First(pair => pair.Key == _wsClientId);
        if (pairForClientId.Key != _wsClientId && pairForClientId.Value.Length > 5)
            throw new Exception("ConnectionIdToSocket should have client ID key and a socket ID, but state was: " +
                                "" + JsonSerializer.Serialize(
                                    await _connectionManager.GetAllConnectionIdsWithSocketId()));
        if (_connectionManager.GetAllSocketIdsWithConnectionId().Result.Keys.Count != 1)
            throw new Exception("SocketToConnectionId should have 1 value, but state was: " +
                                "" + JsonSerializer.Serialize(
                                    await _connectionManager.GetAllSocketIdsWithConnectionId()));
    }

}