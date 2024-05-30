namespace Common;

public interface IReadOnlyTenantProvider
{
    string Tenant { get; }
}