namespace Common;

/// <summary>
/// Implementation of <see cref="IReadOnlyTenantProvider"/> via <see cref="ITenantProvider"/>
/// </summary>
public class ReadOnlyTenantProvider(ITenantProvider tenantProvider) : IReadOnlyTenantProvider
{
    private ITenantProvider tenantProvider = tenantProvider;

    public string Tenant => tenantProvider.Tenant;
}