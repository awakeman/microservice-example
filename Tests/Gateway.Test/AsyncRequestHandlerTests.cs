using AutoFixture.Xunit2;
using Common;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Moq;
using TestUtils;

namespace Gateway.Test;

public class AsyncRequestHandlerTests
{
    [Theory]
    [InlineAutoMoqData("/users/")]
    [InlineAutoMoqData("/settings/")]
    public async Task PublishesToMessageBusIfAsync(
            string path,
            string tenant,
            string body,
            [Frozen] Mock<IReadOnlyTenantProvider> mockTenantProvider,
            [Frozen] Mock<IBus> mockBus,
            Mock<ISendEndpoint> mockSendEndpoint,
            Mock<RequestDelegate> mockRequestDelegate,
            AsyncRequestHandler sut)
    {
        mockTenantProvider.SetupGet(t => t.Tenant).Returns(tenant);
        mockBus.Setup(b => b.GetSendEndpoint(It.IsAny<Uri>())).ReturnsAsync(mockSendEndpoint.Object);

        var content = new StringContent(body);
        var ctx = new DefaultHttpContext() ;
        ctx.Request.Body = content.ReadAsStream();
        ctx.Request.Path = path;
        ctx.Request.Method = "POST";
        ctx.Request.Headers.Append("X-Async", "yes");
        
        await sut.InvokeAsync(ctx, mockRequestDelegate.Object);
        
        Assert.Equal(202, ctx.Response.StatusCode);

        mockRequestDelegate.Verify(r => r(It.IsAny<HttpContext>()), Times.Never);
        mockSendEndpoint.Verify(e => e.Send(new Message { Body = body, Tenant = tenant}, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Theory]
    [AutoMoqData]
    public async Task CallsNextIfUnknownRouteAsync(
            string path,
            string body,
            Mock<RequestDelegate> mockRequestDelegate,
            AsyncRequestHandler sut)
    {
        var content = new StringContent(body);
        var ctx = new DefaultHttpContext() ;
        ctx.Request.Body = content.ReadAsStream();
        ctx.Request.Path = $"/{path}";
        ctx.Request.Method = "POST";
        ctx.Request.Headers.Append("X-Async", "yes");
        
        await sut.InvokeAsync(ctx, mockRequestDelegate.Object);
        

        mockRequestDelegate.Verify(r => r(It.IsAny<HttpContext>()), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task CallsNextIfNotPost(
            string body,
            Mock<RequestDelegate> mockRequestDelegate,
            AsyncRequestHandler sut)
    {
        var content = new StringContent(body);
        var ctx = new DefaultHttpContext() ;
        ctx.Request.Body = content.ReadAsStream();
        ctx.Request.Path = $"/users";
        ctx.Request.Method = "GET";
        ctx.Request.Headers.Append("X-Async", "yes");
        
        await sut.InvokeAsync(ctx, mockRequestDelegate.Object);

        mockRequestDelegate.Verify(r => r(It.IsAny<HttpContext>()), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task CallsNextIfNoHeader(
            string body,
            Mock<RequestDelegate> mockRequestDelegate,
            AsyncRequestHandler sut)
    {
        var content = new StringContent(body);
        var ctx = new DefaultHttpContext() ;
        ctx.Request.Body = content.ReadAsStream();
        ctx.Request.Path = $"/users";
        ctx.Request.Method = "GET";
        
        await sut.InvokeAsync(ctx, mockRequestDelegate.Object);

        mockRequestDelegate.Verify(r => r(It.IsAny<HttpContext>()), Times.Once);
    }
}