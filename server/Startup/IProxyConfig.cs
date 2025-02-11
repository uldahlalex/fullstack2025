namespace Startup;

public interface IProxyConfig
{
    void StartProxyServer(int publicPort, int restPort, int wsPort);
}