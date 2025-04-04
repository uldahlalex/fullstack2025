using System.ComponentModel.DataAnnotations;
using Application.Models;
using Core.Domain;
using NUnit.Framework;
using Startup.Tests.TestUtils;

namespace Startup.Tests.ObjectValidationTests;

public class AppOptionsValidator() : ApiTestBase(new ApiTestBaseConfig
{
    UseTestContainer = false,
    MockWebSocketService = true,
    MockMqtt = true
})
{
    [Test]
    public Task AppOptionsValidatorThrowsException()
    {
        var opts = new AppOptions();
        var context = new ValidationContext(opts, null, null);
        Assert.Throws<ValidationException>(() => Validator.ValidateObject(opts, context));
        return Task.CompletedTask;
    }

    [Test]
    public Task AppOptionsValidatorAcceptsValidAppOptions()
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
        return Task.CompletedTask;
    }
}