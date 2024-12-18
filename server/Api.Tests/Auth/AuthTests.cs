using System.Net;
using Api.Rest.Controllers;
using Application.Models.Dtos;
using Application.Models.Entities;
using Application.Models.Enums;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Api.Tests.Auth;

public class AuthTests(ITestOutputHelper testOutputHelper) : ApiTestBase(testOutputHelper)
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
            new AuthRequestDto()
            {
                Username = TestUsername,
                Password = TestPassword
            });
        Assert.Equal(HttpStatusCode.OK, response.HttpResponseMessage.StatusCode);
        Assert.NotNull(response.Object.Jwt);
    }


    [Fact]
    public async Task Login_Can_Login_And_Return_Jwt()
    {
   
        using var scope = Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    
        ctx.Players.Add(new Player
        {
            FullName = TestUsername,
            Activated = true,
            CreatedAt = DateTime.UtcNow,
            Email = TestUsername,
            Role = Roles.User.ToString(),
            Salt = TestSalt,
            Hash = TestHash
               });
        await ctx.SaveChangesAsync();

        var client = CreateClient();
        var response = await client.PostAsJsonAsync<AuthResponseDto>(AuthController.LoginRoute,
            new AuthRequestDto()
            {
                Username = TestUsername,
                Password = TestPassword
            });
        Assert.Equal(HttpStatusCode.OK, response.HttpResponseMessage.StatusCode);
    }
}