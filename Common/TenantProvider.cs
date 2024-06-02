/// <summary>
/// Implementation of <see cref="ITenantProvider"/>
/// </summary>
public class TenantProvider : ITenantProvider
{
    public string Tenant { get; set; } = string.Empty;
}