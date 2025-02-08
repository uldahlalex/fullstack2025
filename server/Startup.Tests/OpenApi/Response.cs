using NSwag;
using Xunit.Abstractions;

namespace Startup.Tests.OpenApi;

public class OpenApiTests(ITestOutputHelper outputHelper) : ApiTestBase(outputHelper)
{
    [Fact]
    public async Task CanGetJsonResponseFromOpenApi()
    {
        var response = await CreateClient().GetAsync("/openapi/myapi.json");
        var document = await OpenApiDocument.FromJsonAsync(await response.Content.ReadAsStringAsync());
        Assert.True(document.Paths.Count > 0);
    }
}