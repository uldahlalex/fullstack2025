using Application.Models.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class AuthController(ISecurityService securityService) : ControllerBase
{
    public const string ControllerRoute = "api/auth/";

    public const string LoginRoute = ControllerRoute + nameof(Login);


    [HttpPost]
    [Route(LoginRoute)]
    public ActionResult Login([FromBody] AuthRequestDto dto)
    {
        return Ok(securityService.Login(dto));
    }


    public const string RegisterRoute = ControllerRoute + nameof(Register);
    [Route(RegisterRoute)]
    [HttpPost]
    public ActionResult Register([FromBody] AuthRequestDto dto)
    {
        return Ok(securityService.Register(dto));
    }
}