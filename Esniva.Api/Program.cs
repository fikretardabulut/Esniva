using Esniva.Infrastructure.Persistence;
using Esniva.MultiTenancy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EsnivaDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<TenantProvider>();
builder.Services.AddScoped<ITenantProvider>(sp => sp.GetRequiredService<TenantProvider>());

var app = builder.Build();

app.UseMiddleware<TenantMiddleware>();

app.MapGet("/", () => "Esniva API Running");
app.MapGet("/api/tenant", (ITenantProvider tenantProvider) =>
{
    var tenant = tenantProvider.CurrentTenant;
    if (tenant == null)
        return Results.NotFound("Tenant not found");

    return Results.Ok(tenant);
});
app.Run();