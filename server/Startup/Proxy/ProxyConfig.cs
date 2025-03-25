using System.Net;
using WebSocketProxy;
using Host = WebSocketProxy.Host;

namespace Startup.Proxy;

public class CustomProxy : TcpProxyConfiguration
{
    //public Host MqttHost { get; set; }
}


public class ProxyConfig(ILogger<ProxyConfig> logger) : IProxyConfig
{
    public void StartProxyServer(int publicPort, int restPort, int wsPort//, int mqttPort
    )
    {
        var proxyConfiguration = new CustomProxy()
        {
            PublicHost = new Host
            {
                IpAddress = IPAddress.Parse("0.0.0.0"),
                Port = publicPort
            },
            HttpHost = new Host
            {
                IpAddress = IPAddress.Loopback,
                Port = restPort
            },
            WebSocketHost = new Host
            {
                IpAddress = IPAddress.Loopback,
                Port = wsPort
            },
          // MqttHost = new Host
          // {
          //     IpAddress = IPAddress.Loopback,
          //     Port = mqttPort
          // }
        };
        new TcpProxyServer(proxyConfiguration).Start();
    }
}