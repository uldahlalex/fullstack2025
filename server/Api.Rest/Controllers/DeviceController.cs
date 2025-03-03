using System.Text.Json;
using Api.Rest.Extensions;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Mqtt;
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
    public ActionResult ChangePreferencesForDevice()
    {
        //securityService.VerifyJwtOrThrow(HttpContext.GetJwt());
        mqttClientService.PublishAsync("device/A/changePreferences", JsonSerializer.Serialize(new
        {
            units = "Celcius",
            interval = 2000
        }));
        var dto = new
        {
            eventType = "ServerSpams",
            message = "loool"
        };
        connectionManager.BroadcastToTopic("messages", dto);
        var result = new { key = "value" };
        return Ok(result);
    }
}