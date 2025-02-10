using System.ComponentModel.DataAnnotations;
using Application.Models;

namespace Startup.Tests.ObjectValidationTests;

public class AppOptionsValidator(ITestOutputHelper outputHelper) : ApiTestBase(outputHelper, new ApiTestBaseConfig
{
    MockRelationalDatabase = true,
    MockWebSocketService = true,
    MockMqtt = true
})
{
    [Fact]
    public async Task AppOptionsValidatorThrowsException()
    {
        var opts = new AppOptions();
        var context = new ValidationContext(opts, null, null);
        Assert.Throws<ValidationException>(() => Validator.ValidateObject(opts, context));
    }

    [Fact]
    public async Task AppOptionsValidatorAcceptsValidAppOptions()
    {
        var opts = new AppOptions
        {
            DbConnectionString = "abc",
            JwtSecret = "abc",
            MQTT_BROKER_HOST = "abc",
            MQTT_PASSWORD = "abc",
            MQTT_USERNAME = "abc",
            REDIS_HOST = "abc",
            REDIS_PASSWORD = "abc",
            REDIS_USERNAME = "abc",
            RunDbInTestContainer = true,
            Seed = true
        };
        var context = new ValidationContext(opts, null, null);
        Validator.ValidateObject(opts, context); //Does not throw
    }
}