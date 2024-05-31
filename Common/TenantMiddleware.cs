using System.Net;
using System.Text.RegularExpressions;
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
        
        try
        {
            var item = tenant.Single()?.Trim()!;

            var rg = new Regex(@"^[a-zA-Z0-9]{6}$");
            if (!rg.IsMatch(item))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("X-Tenant header must be a 6 character alphanumeric string");
                return;
            }

            tenantProvider.Tenant = item;
        }
        catch (InvalidOperationException)
        {

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync("Multiple X-Tenant headers supplied");
            return;
        }

        await next(context);
    }
}