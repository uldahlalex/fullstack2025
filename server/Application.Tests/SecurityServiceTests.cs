using Application.Interfaces;
using Application.Interfaces.Infrastructure.Postgres;
using Application.Models;
using Application.Services;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Application.Tests;

public class SecurityServiceTests
{
    private readonly ISecurityService _service;


    public SecurityServiceTests()
    {
        var mockAppOptions = new Mock<IOptionsMonitor<AppOptions>>();
        mockAppOptions.SetupGet(x => x.CurrentValue).Returns(new AppOptions
        {
            JwtSecret = ""
        });
        var mockRepo = new Mock<IDataRepository>();
        //mockRepo.Setup(x => x.GetDomainModels()).Returns()
        _service = new SecurityService(mockAppOptions.Object, mockRepo.Object);
    }

    [Test]
    public async Task Hash_Can_Correctly_Hash_A_String()
    {
        var hash = _service.HashPassword("");
    }
}