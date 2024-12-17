using System.Net;
using Api.Rest.Controllers;
using Microsoft.AspNetCore.Mvc;
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
     
            var response = await UserHttpClient.GetStringAsync("/api/secured");
            //log the body
            var body =  response;




            // Assert.Equal(HttpStatusCode.Unauthorized,  response.HttpResponseMessage.StatusCode);
            // Assert.NotEmpty( response.Object.Title);

    }
    
    
    
    
}