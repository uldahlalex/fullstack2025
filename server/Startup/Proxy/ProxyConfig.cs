using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using WebSocketProxy;
using Host = WebSocketProxy.Host;

namespace Startup.Proxy;

public class CustomProxy : TcpProxyConfiguration
{
    public Host MqttHost { get; set; }
}

public class ProxyConfig : IProxyConfig
{
    private readonly ILogger<ProxyConfig> _logger;

    public ProxyConfig(ILogger<ProxyConfig> logger)
    {
        _logger = logger;
    }

    private bool IsPortAvailable(IPAddress ipAddress, int port)
    {
        try
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(ipAddress, port));
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    public void StartProxyServer(int publicPort, int restPort, int wsPort, int mqttPort)
    {
        _logger.LogInformation("Checking availability of ports before starting proxy server");
        
        var publicIp = IPAddress.Parse("0.0.0.0");
        var loopbackIp = IPAddress.Loopback;
        
        if (!IsPortAvailable(publicIp, publicPort))
            _logger.LogError("Public port {PublicPort} is already in use", publicPort);
        
        if (!IsPortAvailable(loopbackIp, restPort))
            _logger.LogError("REST port {RestPort} is already in use", restPort);
        
        if (!IsPortAvailable(loopbackIp, wsPort))
            _logger.LogError("WebSocket port {WsPort} is already in use", wsPort);
        
        if (!IsPortAvailable(loopbackIp, mqttPort))
            _logger.LogError("MQTT port {MqttPort} is already in use", mqttPort);

        var proxyConfiguration = new CustomProxy()
        {
            PublicHost = new Host
            {
                IpAddress = publicIp,
                Port = publicPort
            },
            HttpHost = new Host
            {
                IpAddress = loopbackIp,
                Port = restPort
            },
            WebSocketHost = new Host
            {
                IpAddress = loopbackIp,
                Port = wsPort
            },
            MqttHost = new Host
            {
                IpAddress = loopbackIp,
                Port = mqttPort
            }
        };

        try
        {
            _logger.LogInformation("Starting proxy server with configuration: Public={PublicPort}, REST={RestPort}, WebSocket={WsPort}, MQTT={MqttPort}", 
                publicPort, restPort, wsPort, mqttPort);
            
            new TcpProxyServer(proxyConfiguration).Start();
            
            _logger.LogInformation("Proxy server successfully started");
        }
        catch (SocketException ex)
        {
            _logger.LogError(ex, "Failed to start proxy server due to socket exception. Error code: {ErrorCode}", ex.ErrorCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start proxy server due to an unexpected error");
            throw;
        }
    }
}