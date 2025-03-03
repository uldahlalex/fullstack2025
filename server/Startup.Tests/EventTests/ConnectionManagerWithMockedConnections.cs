using Api;
using Api.WebSockets;
using Fleck;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Startup;
using Startup.Tests.TestUtils;
using WebSocketBoilerplate;

namespace NUnit;

[TestFixture(typeof(DictionaryConnectionManager))]
public class ConnectionManagerWithMockedConnections(Type connectionManagerType) : ApiTestBase
{

    [Test]
    public async Task OnConnect_Can_Add_Socket_And_Client_To_Storage()
    {


        var connectionId = Guid.NewGuid().ToString();
        var socketId = Guid.NewGuid();
        var wsMock = new Mock<IWebSocketConnection>();
        wsMock.SetupGet(ws => ws.ConnectionInfo.Id).Returns(socketId);
        var ws = wsMock.Object;

        // act
        await _connectionManager.OnOpen(ws, connectionId);

        // assert
        if (!_connectionManager.GetAllConnectionIdsWithSocketId().Result.Values.Contains(ws.ConnectionInfo.Id.ToString()))
            throw new Exception("The dictionary should contain the websocket with guid " + ws.ConnectionInfo.Id +
                                " as the first value");
        if (!_connectionManager.GetAllSocketIdsWithConnectionId().Result.Values.Contains(connectionId))
            throw new Exception("The dictionary " + nameof(_connectionManager.GetAllSocketIdsWithConnectionId) +
                                " should contain the connectionId with guid " + connectionId +
                                " as the first value");
    }

    [Test]
    public async Task OnClose_Can_Remove_Socket_And_Client_From_Storage()
    {

        var connectionId = Guid.NewGuid().ToString();
        var socketId = Guid.NewGuid();
        var wsMock = new Mock<IWebSocketConnection>();
        wsMock.SetupGet(ws => ws.ConnectionInfo.Id).Returns(socketId);
        var ws = wsMock.Object;
        await _connectionManager.OnOpen(ws, connectionId);

        // act
        await _connectionManager.OnClose(ws, connectionId);

        // assert
        if (_connectionManager.GetAllConnectionIdsWithSocketId().Result.Values.Contains(ws.ConnectionInfo.Id.ToString()))
            throw new Exception("The dictionary should not contain the websocket with guid " + ws.ConnectionInfo.Id);
        if (_connectionManager.GetAllSocketIdsWithConnectionId().Result.Values.Contains(connectionId))
            throw new Exception("The dictionary should not contain the connectionId with guid " + connectionId);
    }

    [Test]
    public async Task AddToTopic_Correctly_Adds_Member_To_Topic()
    {
        var randomTopic = Guid.NewGuid().ToString();
        var randomUser = Guid.NewGuid().ToString();

        await _connectionManager.AddToTopic(randomTopic, randomUser);

        var members = await _connectionManager.GetMembersFromTopicId(randomTopic);
        if (!members.Contains(randomUser))
            throw new Exception("The topic " + randomTopic + " should contain the user " + randomUser);

        var topics = await _connectionManager.GetTopicsFromMemberId(randomUser);
        if (!topics.Contains(randomTopic))
            throw new Exception("The user " + randomUser + " should be in the topic " + randomTopic);
    }

    [Test]
    public async Task RemoveFromTopic_Correctly_Removes_Member_From_Topic()
    {
        var randomTopic = Guid.NewGuid().ToString();
        var randomUser = Guid.NewGuid().ToString();

        await _connectionManager.AddToTopic(randomTopic, randomUser);
        await _connectionManager.RemoveFromTopic(randomTopic, randomUser);

        var members = await _connectionManager.GetMembersFromTopicId(randomTopic);
        if (members.Contains(randomUser))
            throw new Exception("The topic " + randomTopic + " should not contain the user " + randomUser);
        var topicsFromMemberId = await _connectionManager.GetTopicsFromMemberId(randomUser);
        if (topicsFromMemberId.Contains(randomTopic))
            throw new Exception("The user " + randomUser + " should not be in the topic " + randomTopic);
    }
}