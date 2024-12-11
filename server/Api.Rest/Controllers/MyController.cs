
using core;
using Microsoft.AspNetCore.Mvc;

namespace restapi.Controllers;

[ApiController]
[Route("api")]
public class MyController(IServiceLogic service) : ControllerBase
{
    
    public ActionResult DoSomething()
    {
        return Ok(service.GetDomainModels());
    }
    
}