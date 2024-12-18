using Api.Rest.ActionFilters;
using Application.Models.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class AuthController(ISecurityService securityService) : ControllerBase
{
    public const string ControllerRoute = "api/auth/";

    public const string LoginRoute = ControllerRoute + nameof(Login);


    public const string RegisterRoute = ControllerRoute + nameof(Register);


    public const string SecuredRoute = ControllerRoute + nameof(Secured);


    [HttpPost]
    [Route(LoginRoute)]
    public ActionResult<AuthResponseDto> Login([FromBody] AuthRequestDto dto)
    {
        return Ok(securityService.Login(dto));
    }

    [Route(RegisterRoute)]
    [HttpPost]
    public ActionResult<AuthResponseDto> Register([FromBody] AuthRequestDto dto)
    {
        return Ok(securityService.Register(dto));
    }

    [HttpGet]
    [TypeFilter(typeof(VerifyJwt))]
    [Route(SecuredRoute)]
    public ActionResult Secured()
    {
        return Ok("You are authorized to see this message");
    }
}