using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Startup;

public class CustomWebTestFixture : IDisposable
{
    private readonly TestServer _testServer;
    private readonly HttpClient _client;

    public CustomWebTestFixture()
    {
        _testServer = new TestServer(new WebHostBuilder()
            .UseTestServer()
            .UseEnvironment("Testing")
            .ConfigureAppConfiguration((context, config) =>
            {
                // Add any test-specific configuration here
            })
            .ConfigureServices((context, services) =>
            {
                // Configure all services in one place
                Program.ConfigureServices(services, context.Configuration, context.HostingEnvironment);
                
                // Replace IProxyConfig with mock
                var proxy = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IProxyConfig));
                if (proxy != null)
                {
                    services.Remove(proxy);
                }
                services.AddSingleton<IProxyConfig, MockProxyConfig>();
            })
            .Configure(app =>
            {
                var webApp = (WebApplication)app;
                Program.ConfigureMiddleware(webApp);
            }));

        _client = _testServer.CreateClient();
    }

    public HttpClient Client => _client;

    public T GetService<T>() where T : class
    {
        return _testServer.Services.GetService<T>();
    }

    public void Dispose()
    {
        _client?.Dispose();
        _testServer?.Dispose();
    }
}

public class ApiTests
{
    private readonly CustomWebTestFixture _fixture;

    public ApiTests()
    {
        _fixture = new CustomWebTestFixture();
    }

    [Fact]
    public async Task TestAcceptanceEndpoint()
    {
        var response = await _fixture.Client.GetAsync("/Acceptance");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Accepted", content);
    }
}