using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common;

public class TenantMiddleware(
    ITenantProvider tenantProvider,
    ILogger<TenantMiddleware> logger)
    : IMiddleware
{
    private ITenantProvider tenantProvider = tenantProvider;
    private ILogger logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue("X-Tenant", out var tenant))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync("X-Tenant header is required");
            return;
        }

        tenantProvider.Tenant = tenant.First()!;

        await next(context);
    }
}