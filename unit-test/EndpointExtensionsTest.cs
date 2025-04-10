using Microsoft.AspNetCore.Builder;
using server;
using server.Extensions;

namespace unit_test;

public class EndpointExtensionsTest
{
    [Fact]
    public void RequireRoleTest()
    {
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        var result = app.MapGet("/api", () => "Hello World!").RequireRole(Role.ADMIN);

        Assert.IsType<RouteHandlerBuilder>(result);
        var message = result.ToString();
        Assert.Equal("Microsoft.AspNetCore.Builder.RouteHandlerBuilder", message);
        
        // how do I test if RouteHandlerBuilder worked or not??
        
        // THIS TEST WAS A FAILURE!
        // (but it still passed (╯°□°)╯︵ bad test)
    }
}
