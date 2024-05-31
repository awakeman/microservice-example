using Microsoft.AspNetCore.Http;
using Moq;
using TestUtils;

namespace Common.Test;

public class TenantMiddlewareTests
{
    [Theory]
    [InlineAutoMoqData("abcdef")] // all letters
    [InlineAutoMoqData("123456")] // all numbers
    [InlineAutoMoqData("abc456")] // mix of letters and numbers
    [InlineAutoMoqData("aBc456")] // mixed case
    [InlineAutoMoqData("  aBc456 ")] // leading and trailing whitespace
    public async Task ValidTenantHeaderCallsNext(
            string tenant,
            Mock<RequestDelegate> mockNext,
            TenantMiddleware sut)
    {
        DefaultHttpContext ctx = new ();
        ctx.Request.Headers.Append("X-Tenant", tenant);

        await sut.InvokeAsync(ctx, mockNext.Object);
        
        mockNext.Verify(n => n(ctx), Times.Once);
    }

    [Theory]
    [InlineAutoMoqData("abc")] // too short
    [InlineAutoMoqData("123 45")] // whitespace in the middle
    [InlineAutoMoqData("ab<456")] // symbol
    [InlineAutoMoqData("xyz12345")] // too long
    public async Task InvalidTenantHeaderErrors(
            string tenant,
            Mock<RequestDelegate> mockNext,
            TenantMiddleware sut)
    {
        DefaultHttpContext ctx = new ();
        ctx.Request.Headers.Append("X-Tenant", tenant);

        await sut.InvokeAsync(ctx, mockNext.Object);
        
        Assert.Equal(400, ctx.Response.StatusCode);
        
        mockNext.Verify(n => n(ctx), Times.Never);
    }

    [Theory]
    [AutoMoqData]
    public async Task MissingTenantHeaderErrors(
            Mock<RequestDelegate> mockNext,
            TenantMiddleware sut)
    {
        DefaultHttpContext ctx = new ();

        await sut.InvokeAsync(ctx, mockNext.Object);
        
        Assert.Equal(400, ctx.Response.StatusCode);
        
        mockNext.Verify(n => n(ctx), Times.Never);
    }

    [Theory]
    [AutoMoqData]
    public async Task MultipleTenantHeaderErrors(
            string[] tenants,
            Mock<RequestDelegate> mockNext,
            TenantMiddleware sut)
    {
        DefaultHttpContext ctx = new ();

        foreach (var tenant in tenants)
        {
            ctx.Request.Headers.Append("X-Tenant", tenant);
        }

        await sut.InvokeAsync(ctx, mockNext.Object);
        
        Assert.Equal(400, ctx.Response.StatusCode);
        
        mockNext.Verify(n => n(ctx), Times.Never);
    }
}