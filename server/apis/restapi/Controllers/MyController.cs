
using core;
using Microsoft.AspNetCore.Mvc;

namespace restapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MyController(IServiceLogic service) : ControllerBase
{
    
    public ActionResult DoSomething()
    {
        return Ok(service.GetDomainModels());
    }
    
}