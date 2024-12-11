using Microsoft.AspNetCore.Mvc;
using service.Interfaces;

namespace Api.Rest.Controllers;

[ApiController]
[Route("api")]
public class MyController(IServiceLogic service) : ControllerBase
{
    public ActionResult DoSomething()
    {
        return Ok(service.GetDomainModels());
    }
}