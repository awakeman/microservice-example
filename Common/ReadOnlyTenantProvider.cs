namespace Common;

public class ReadOnlyTenantProvider(ITenantProvider tenantProvider) : IReadOnlyTenantProvider
{
    private ITenantProvider tenantProvider = tenantProvider;

    public string Tenant => tenantProvider.Tenant;
}