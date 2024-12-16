using System.Security.Authentication;
using Application.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Rest.ActionFilters;

public class VerifyJwt : IActionFilter
{
    private readonly ISecurityService _securityService;

    public VerifyJwt(ISecurityService securityService)
    {
        _securityService = securityService;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (token is null) throw new AuthenticationException("No token provided");
        _securityService.VerifyJwt(token);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Implementation not needed
    }
}