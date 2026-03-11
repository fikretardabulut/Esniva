namespace Esniva.MultiTenancy;

public class TenantContext
{
    public Guid TenantId { get; set; }

    public string TenantName { get; set; } = default!;

    public string Subdomain { get; set; } = default!;
}