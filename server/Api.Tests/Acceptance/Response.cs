using System.Net;
using Microsoft.AspNetCore.Http;
using NSwag;
using Xunit.Abstractions;

namespace Api.Tests.Acceptance;

public class OpenApiTests(ITestOutputHelper outputHelper) : ApiTestBase(outputHelper)
{
    [Fact]
    public async Task CanGetJsonResponseFromOpenApi()
    {
        var response = await CreateClient().GetAsync("/openapi/myapi.json");
        var document = await OpenApiDocument.FromJsonAsync(await response.Content.ReadAsStringAsync());
        Assert.True(document.Paths.Count > 0);
    }

    [Fact]
    public async Task AcceptanceTest()
    {
        var client = CreateClient();
        var response = await client.GetAsync("/acceptance");
        Assert.Equal("Accepted", await response.Content.ReadAsStringAsync());
    }
}