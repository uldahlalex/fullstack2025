using Api.Rest.ActionFilters;
using Application.Interfaces;
using Application.Models;
using Application.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Rest.Controllers;

[ApiController]
[Route("api")]
public class MyController(IServiceLogic service, IOptionsMonitor<AppOptions> optionsMonitor) : ControllerBase
{
    public ActionResult<IEnumerable<Board>> DoSomething([FromQuery] string param)
    {
        return Ok(service.GetDomainModels());
    }

    [Route("secured")]
    [TypeFilter(typeof(VerifyJwt))]
    public ActionResult SecuredEndpoint()
    {
        return Ok("You are authorized to see this message");
    }
}