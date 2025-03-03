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



namespace NUnit;

[TestFixture(typeof(DictionaryConnectionManager))]
public class ConnectionWithWsClient(Type connectionManagerType) : WebApplicationFactory<Program>
{
    private ILogger<ConnectionWithWsClient> _logger;
    private HttpClient _httpClient;
    private MyDbContext _dbContext;
    private IConnectionManager<IWebSocketConnection, BaseDto> _connectionManager;
    private string _wsClientId;
    private WsRequestClient _wsClient;
    private IServiceScope _scope;
    
    [SetUp]
    public async Task Setup()
    {
        _httpClient = CreateClient();

        //Singletons
        _logger = Services.GetRequiredService<ILogger<ConnectionWithWsClient>>();
        _connectionManager =  Services.GetRequiredService<IConnectionManager<IWebSocketConnection, BaseDto>>();

        //Scoped services
        using var scope = Services.CreateScope();
        {
            _scope = Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<MyDbContext>();
        }
        
        var wsPort = Environment.GetEnvironmentVariable("PORT");
        if (string.IsNullOrEmpty(wsPort)) throw new Exception("Environment variable PORT is not set");
        _wsClientId = Guid.NewGuid().ToString();
        var url = "ws://localhost:" + wsPort + "?id=" + _wsClientId;
        _wsClient = new WsRequestClient(
            new[] { typeof(ClientWantsToEchoDto).Assembly },
            url
        );
        await _wsClient.ConnectAsync();
        await Task.Delay(1000);
    }

 


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