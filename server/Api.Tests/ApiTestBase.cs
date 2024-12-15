using infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PgCtx;
using Startup;
using Xunit.Abstractions;

public class ApiTestBase : WebApplicationFactory<Program>
{   
    private readonly ITestOutputHelper _outputHelper;
    public PgCtxSetup<MyDbContext> PgCtxSetup;
    public HttpClient UserHttpClient { get; private set; }
    public HttpClient AdminHttpClient { get; private set; }
    public string UserJwt { get; private set; }
    public string AdminJwt { get; private set; }
    public IServiceProvider ApplicationServices { get; private set; }

    public ApiTestBase(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        PgCtxSetup = new PgCtxSetup<MyDbContext>();
        
        // Now the sequence is clear:
        SetupTestServer();    // 1. Configure and start the test server
        SetupHttpClients();   // 2. Create and configure HTTP clients
    }

    private void SetupTestServer()
    {
        // This will trigger ConfigureWebHost and initialize the server
        ApplicationServices = Services.CreateScope().ServiceProvider;
    }

    private void SetupHttpClients()
    {
        UserHttpClient = CreateClient();
        AdminHttpClient = CreateClient();

        // Setup authentication if needed
        // var tokenService = ApplicationServices.GetRequiredService<ITokenClaimsService>();
        // UserJwt = await tokenService.GetTokenAsync("testuser");
        // AdminJwt = await tokenService.GetTokenAsync("adminuser");

        // Configure client headers
        // UserHttpClient.DefaultRequestHeaders.Authorization = 
        //     new AuthenticationHeaderValue("Bearer", UserJwt);
        // AdminHttpClient.DefaultRequestHeaders.Authorization = 
        //     new AuthenticationHeaderValue("Bearer", AdminJwt);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Your existing ConfigureWebHost implementation
    }
}