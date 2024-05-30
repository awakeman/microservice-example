using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Common;

public static class TenantExtensions
{
    public static IServiceCollection AddTenantHandling(this IServiceCollection services) => services
            .AddScoped<ITenantProvider, TenantProvider>()
            .AddScoped<IReadOnlyTenantProvider, ReadOnlyTenantProvider>()
            .AddScoped<TenantMiddleware>();

    public static IApplicationBuilder UseTenantHandling(this IApplicationBuilder app) => app.UseMiddleware<TenantMiddleware>();
}
