using System.ComponentModel.DataAnnotations;
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
    [Route(nameof(AdminWantsToChangePreferencesForDevice))]
    public ActionResult AdminWantsToChangePreferencesForDevice([FromBody] AdminWantsToChangePreferencesForDeviceDto dto)
    {
        //securityService.VerifyJwtOrThrow(HttpContext.GetJwt());
        var serialized = JsonSerializer.Serialize(dto);
        mqttClientService.PublishAsync("device/" + dto.DeviceId + "/changePreferences", serialized);
        return Ok();
    }
}