using System.Net;
using Xunit.Abstractions;

namespace Api.Tests.Auth;

public class AuthTests(ITestOutputHelper testOutputHelper) : ApiTestBase(testOutputHelper)
{
    [Fact]
    public async Task RouteWithNoAuth_Can_Be_Accessed()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/acceptance");
        Assert.Equal("Accepted", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task SecuredRouteIsBlocedWithoutJwt()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/api/secured");
        Assert.Equal(HttpStatusCode.Unauthorized,  response.StatusCode);
    }
    
    
    
    
}