using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using service.Interfaces;
using service.Models;
using service.Types;

namespace Api.Rest.Controllers;

[ApiController]
[Route("api")]
public class MyController(IServiceLogic service, IOptionsMonitor<AppOptions> optionsMonitor) : ControllerBase
{
    public ActionResult<IEnumerable<Board>> DoSomething([FromQuery]string param)
    {
        return Ok(service.GetDomainModels());
    }
}
