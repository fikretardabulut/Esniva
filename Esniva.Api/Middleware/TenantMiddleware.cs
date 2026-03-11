using Esniva.Infrastructure.Persistence;
using Esniva.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        TenantProvider tenantProvider,
        EsnivaDbContext db)
    {
        var host = context.Request.Host.Host;

        // Eğer host içinde port varsa ayıkla
        var tenantName = host.Split(':')[0];

        var tenant = await db.Tenants
            .Where(x => x.Subdomain == tenantName) // artık host adı ile eşleşecek
            .FirstOrDefaultAsync();

        if (tenant == null)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Tenant not found");
            return;
        }

        tenantProvider.SetTenant(new TenantContext
        {
            TenantId = tenant.Id,
            TenantName = tenant.Name,
            Subdomain = tenant.Subdomain
        });

        await _next(context);
    }
}