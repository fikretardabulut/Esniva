namespace Esniva.MultiTenancy;

public class TenantProvider : ITenantProvider
{
    public TenantContext? CurrentTenant { get; private set; }

    public void SetTenant(TenantContext tenant)
    {
        CurrentTenant = tenant;
    }
}