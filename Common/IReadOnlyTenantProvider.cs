namespace Common;

/// <summary>
/// Interface <c>IReadOnlyTenantProvider</c> readonly interface to supply Tenenat information
/// </summary>
/// <remarks>
/// <see cref="ITenantProvider" /> for a rw interface
/// </remarks>
public interface IReadOnlyTenantProvider
{
    string Tenant { get; }
}