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

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue("X-Tenant", out var tenant))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.WriteAsync("X-Tenant header is required");
        }

        logger.LogInformation("Header found: {Tenant}", tenant);

        tenantProvider.Tenant = tenant.First()!;

        return next(context);
    }
}