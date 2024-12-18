using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Rest.Controllers;

[ApiController]
public class MyController(IServiceLogic service, IOptionsMonitor<AppOptions> optionsMonitor) : ControllerBase
{
}