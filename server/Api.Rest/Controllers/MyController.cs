using Api.Rest.ActionFilters;
using Application.Interfaces;
using Application.Models;
using Application.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Rest.Controllers;

[ApiController]
public class MyController(IServiceLogic service, IOptionsMonitor<AppOptions> optionsMonitor) : ControllerBase
{
    public const string DoSomethingRoute = "api/mycontroller/dosomething";
    [Route(DoSomethingRoute)]
    [HttpGet]
    public ActionResult<IEnumerable<Board>> DoSomething([FromQuery] string param)
    {
        return Ok(service.GetDomainModels());
    }

    public const string SecuredRoute = "api/secured";

    [HttpGet("secured")]  // Combines with base route to make "api/secured"
    [TypeFilter(typeof(VerifyJwt))]
    public ActionResult SecuredEndpoint()
    {
        return Ok("You are authorized to see this message");
    }
}