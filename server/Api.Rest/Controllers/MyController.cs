using Api.Rest.Extensions;
using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Rest.Controllers;

[ApiController]
public class MyController(
    IServiceLogic service,
    ISecurityService securityService,
    IOptionsMonitor<AppOptions> optionsMonitor) : ControllerBase
{
    [Route("/do")]
    public ActionResult Do()
    {
        var claims = securityService.VerifyJwtOrThrow(HttpContext.GetJwt());
        return Ok(service.GetDomainModels(claims));
    }
}