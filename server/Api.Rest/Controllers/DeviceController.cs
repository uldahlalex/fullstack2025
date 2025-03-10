using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Application.Interfaces;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Application.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Rest.Controllers;



[ApiController]
public class DeviceController(
    IServiceLogic service,
    ISecurityService securityService,
    IDataRepository repository,
    IOptionsMonitor<AppOptions> optionsMonitor,
    IMqttPublisher<AdminWantsToChangePreferencesForDeviceDto> publisher,
    IConnectionManager connectionManager) : ControllerBase
{
    [Route(nameof(AdminWantsToChangePreferencesForDevice))]
    public ActionResult AdminWantsToChangePreferencesForDevice([FromBody] AdminWantsToChangePreferencesForDeviceDto dto)
    {
        //securityService.VerifyJwtOrThrow(HttpContext.GetJwt()); //this is an example of jwt authentication for REST using the same SecurityService as WebSocket API 
        publisher.Publish(dto);
        return Ok();
    }
    
    [Route(nameof(AdminWantsToClearData))]
    public ActionResult<List<Devicelog>> AdminWantsToClearData()
    {
        repository.ClearMetrics();
        var allMetrics = repository.GetAllMetrics();
        return Ok(allMetrics);
    }
}