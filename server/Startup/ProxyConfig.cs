using System.Net;
using WebSocketProxy;
using Host = WebSocketProxy.Host;


namespace Startup;

public interface IProxyConfig
{
    void StartProxyServer();
}

public class ProxyConfig : IProxyConfig
{
    public void StartProxyServer()
    {
      
        var proxyConfiguration = new TcpProxyConfiguration
        {
            PublicHost = new Host
            {
                IpAddress = IPAddress.Parse("0.0.0.0"),
                Port = 8080
            },
            HttpHost = new Host
            {
                IpAddress = IPAddress.Loopback,
                Port = 5000
            },
            WebSocketHost = new Host
            {
                IpAddress = IPAddress.Loopback,
                Port = 8181
            }
        };
        new TcpProxyServer(proxyConfiguration).Start();
    }
}