using System.Text.Json;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Rest.Controllers;

[ApiController]
public class DeviceController(
    IServiceLogic service,
    ISecurityService securityService,
    IOptionsMonitor<AppOptions> optionsMonitor,
    IConnectionManager connectionManager,
    IMqttClientService mqttClientService) : ControllerBase
{
    [Route(nameof(ChangePreferencesForDevice))]
    public ActionResult ChangePreferencesForDevice(string deviceId, int milliseconds)
    {
        //securityService.VerifyJwtOrThrow(HttpContext.GetJwt());
        mqttClientService.PublishAsync("device/" + deviceId + "/changePreferences", JsonSerializer.Serialize(new
        {
            unit = "Celcius",
            interval = milliseconds
        }));
        return Ok();
    }
}