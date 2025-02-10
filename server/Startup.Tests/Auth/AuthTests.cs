using System.Net;
using Api.Rest.Controllers;
using Application.Models.Dtos;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Startup.Tests.Auth;


public class AuthTests(ITestOutputHelper testOutputHelper) : ApiTestBase(testOutputHelper, new ApiTestBaseConfig
{
    MockMqtt = true,
    MockWebSocketService = false
})
{
    private const string TestUsername = "bob@bob.dk";
    private const string TestPassword = "asdASD123,-.";
    private const string TestSalt = "5cbd23b9-0cb4-4afe-8497-c81bc6691a42";

    private const string TestHash =
        "J4SHSN9SKisNBoijKZkNAA5GNWJlO/RNsiXWhoWq2lOpd7hBtmwnqb6bOcxxYP8tEvNRomJunrVkWKNa5W3lXg==";


    [Fact]
    public async Task RouteWithNoAuth_Can_Be_Accessed()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/acceptance");
        Assert.Equal("Accepted", await response.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SecuredRouteIsBlockedWitoutJwt()
    {
        var response = await CreateClient().GetAsync(AuthController.SecuredRoute);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_Can_Register_And_Return_Jwt()
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync<AuthResponseDto>(AuthController.RegisterRoute,
            MockObjects.GetAuthRequestDto());
        Assert.Equal(HttpStatusCode.OK, response.HttpResponseMessage.StatusCode);
        Assert.NotNull(response.Object.Jwt);
    }

    [Fact]
    public async Task Register_With_Short_Pass_Returns_Bad_Request()
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync<ProblemDetails>(
            AuthController.RegisterRoute, new AuthRequestDto
            {
                Username = "bob@bob.dk",
                Password = "a"
            });
        Assert.Equal(HttpStatusCode.BadRequest, response.HttpResponseMessage.StatusCode);
        Assert.True(response.Object.Title!.Length > 1);
    }

    [Fact]
    public async Task Login_Can_Login_And_Return_Jwt()
    {
        using var scope = Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        ctx.Players.Add(MockObjects.GetPlayer());
        await ctx.SaveChangesAsync();

        var client = CreateClient();
        var response = await client.PostAsJsonAsync<AuthResponseDto>(
            AuthController.LoginRoute,
            MockObjects.GetAuthRequestDto());
        Assert.Equal(HttpStatusCode.OK, response.HttpResponseMessage.StatusCode);
    }

    [Fact]
    public async Task Invalid_Login_Gives_Unauthorized()
    {
        using var scope = Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        var player = MockObjects.GetPlayer();
        ctx.Players.Add(player);
        await ctx.SaveChangesAsync();

        var request = MockObjects.GetAuthRequestDto();
        request.Password = request.Password + "a";
        var response = await CreateClient().PostAsJsonAsync<ProblemDetails>(AuthController.LoginRoute,
            request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.HttpResponseMessage.StatusCode);
        Assert.True(response.Object.Title.Length > 1);
    }

    [Fact]
    public async Task Login_For_Non_Existing_User_Is_Unauthorized()
    {
        var response = await CreateClient().PostAsJsonAsync<ProblemDetails>(AuthController.LoginRoute,
            MockObjects.GetAuthRequestDto());
        Assert.Equal(HttpStatusCode.BadRequest, response.HttpResponseMessage.StatusCode);
    }

    [Fact]
    public async Task Register_For_Existing_User_Is_Bad_Request()
    {
        using var scope = Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        var player = MockObjects.GetPlayer();
        ctx.Players.Add(player);
        await ctx.SaveChangesAsync();

        var response = await CreateClient().PostAsJsonAsync<ProblemDetails>(AuthController.RegisterRoute,
            MockObjects.GetAuthRequestDto());
        Assert.Equal(HttpStatusCode.BadRequest, response.HttpResponseMessage.StatusCode);
        Assert.True(response.Object.Title.Length > 1);
    }
}