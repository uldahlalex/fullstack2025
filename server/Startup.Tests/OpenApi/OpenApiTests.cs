using NSwag;

namespace Startup.Tests.OpenApi;

public class OpenApiTests(ITestOutputHelper outputHelper) : ApiTestBase(outputHelper)
{
    [Fact]
    public async Task CanGetJsonResponseFromOpenApi()
    {
        var response = await CreateClient().GetAsync("/swagger/v1/swagger.json");
        var document = await OpenApiDocument.FromJsonAsync(await response.Content.ReadAsStringAsync());
        Assert.True(document.Paths.Count > 0);
    }
}