using Application.Interfaces;
using Application.Interfaces.Infrastructure.Mqtt;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Interfaces.Infrastructure.Websocket;
using Application.Models;
using Application.Models.Dtos;
using Core.Domain;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Rest.Controllers;

[ApiController]
public class DeviceController(
    IDataRepository repository,
    IConnectionManager connectionManager,
    IMqttPublisher publisher) : ControllerBase
{
    [HttpPost]
    [Route(nameof(AdminWantsToChangePreferencesForDevice))]
    public ActionResult AdminWantsToChangePreferencesForDevice([FromBody] 
        AdminWantsToChangePreferencesForDeviceDto dto)
    {
        publisher.Publish(dto, "device/" + dto.DeviceId + "/adminWantsToChangePreferencesForDevice");
        return Ok();
    }

    [HttpDelete]
    [Route(nameof(AdminWantsToClearData))]
    public ActionResult<List<Devicelog>> AdminWantsToClearData()
    {
        repository.ClearMetrics();
        var allMetrics = repository.GetAllMetrics();
        return Ok(allMetrics);
    }
}