namespace Esniva.MultiTenancy;

public interface ITenantProvider
{
    TenantContext? CurrentTenant { get; }
}