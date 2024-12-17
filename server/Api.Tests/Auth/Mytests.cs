// using System.Net;
// using Api.Rest.Controllers;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.AspNetCore.TestHost;
// using Microsoft.Extensions.DependencyInjection;
// using Startup;
//
// namespace Api.Tests.Auth;
//
// public class Mytests : WebApplicationFactory<Program>
// {
//     //configureWebHost
//     protected override void ConfigureWebHost(IWebHostBuilder builder)
//     {
//         builder.ConfigureServices(services =>
//         {
//             var proxy = services.FirstOrDefault(d => d.ServiceType == typeof(IProxyConfig));
//             services.Remove(proxy);
//             services.AddScoped<IProxyConfig, MockProxyConfig>();
//         });
//     }
//     
//     [Fact]
//     public void ApiTest()
//     {
//         // Arrange
//         var client = CreateClient();
//
//         // Act
//         var response = client.GetAsync(MyController.SecuredRoute).Result;
//
//         // Assert
//         Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//     }
// }