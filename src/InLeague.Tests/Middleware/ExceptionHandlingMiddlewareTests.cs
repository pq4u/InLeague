using InLeague.Middleware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;

namespace InLeague.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _mockLogger;
    private readonly Mock<IWebHostEnvironment> _mockEnv;

    public ExceptionHandlingMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        _mockEnv = new Mock<IWebHostEnvironment>();
        _mockEnv.Setup(e => e.EnvironmentName).Returns("Development");
    }

    [Fact]
    public async Task InvokeAsync_NoException_CallsNext()
    {
        var context = new DefaultHttpContext();
        RequestDelegate next = (innerContext) => Task.CompletedTask;
        var middleware = new ExceptionHandlingMiddleware(next, _mockLogger.Object, _mockEnv.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ExceptionInDevelopment_HandlesErrorWithDetails()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        RequestDelegate next = (innerContext) => throw new Exception("Test Exception");
        var middleware = new ExceptionHandlingMiddleware(next, _mockLogger.Object, _mockEnv.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);

        Assert.Equal("Wystąpił błąd serwera", response.GetProperty("error").GetString());
        Assert.Equal("Test Exception", response.GetProperty("message").GetString());
    }

    [Fact]
    public async Task InvokeAsync_ExceptionInProduction_HidesDetails()
    {
        _mockEnv.Setup(e => e.EnvironmentName).Returns("Production");
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        RequestDelegate next = (innerContext) => throw new Exception("Test Exception");
        var middleware = new ExceptionHandlingMiddleware(next, _mockLogger.Object, _mockEnv.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        var response = JsonSerializer.Deserialize<JsonElement>(responseBody);

        Assert.Equal("Wystąpił błąd serwera", response.GetProperty("error").GetString());
        Assert.Equal("Wystąpił nieoczekiwany błąd serwera.", response.GetProperty("message").GetString());
    }
}
