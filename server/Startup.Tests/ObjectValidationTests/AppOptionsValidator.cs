using System.ComponentModel.DataAnnotations;
using Application.Models;
using NUnit.Framework;
using Startup.Tests.TestUtils;

namespace Startup.Tests.ObjectValidationTests;

public class AppOptionsValidator() : ApiTestBase( new ApiTestBaseConfig
{
    MockRelationalDatabase = true,
    MockWebSocketService = true,
    MockMqtt = true
})
{
    [Test]
    public async Task AppOptionsValidatorThrowsException()
    {
        var opts = new AppOptions();
        var context = new ValidationContext(opts, null, null);
        Assert.Throws<ValidationException>(() => Validator.ValidateObject(opts, context));
    }

    [Test]
    public async Task AppOptionsValidatorAcceptsValidAppOptions()
    {
        var opts = new AppOptions
        {
            DbConnectionString = "abc",
            JwtSecret = "abc",
            MQTT_BROKER_HOST = "abc",
            MQTT_PASSWORD = "abc",
            MQTT_USERNAME = "abc",
            Seed = true
        };
        var context = new ValidationContext(opts, null, null);
        Validator.ValidateObject(opts, context); //Does not throw
    }
}