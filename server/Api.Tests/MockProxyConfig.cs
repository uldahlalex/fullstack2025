using Startup;

namespace Api.Tests;

public class MockProxyConfig : IProxyConfig
{
    /// <summary>
    ///     Deliberately don't do anything for the mock proxy
    /// </summary>
    public void StartProxyServer(int publicPort, int restPort, int wsPort)
    {
    }
}