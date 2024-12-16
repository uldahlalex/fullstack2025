using Application.Models.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(ISecurityService securityService) : ControllerBase
{
    [Route("login")]
    [HttpPost]
    public ActionResult Login([FromBody] AuthRequestDto dto)
    {
        return Ok(securityService.Login(dto));
    }

    [Route("register")]
    [HttpPost]
    public ActionResult Register([FromBody] AuthRequestDto dto)
    {
        return Ok(securityService.Register(dto));
    }
}