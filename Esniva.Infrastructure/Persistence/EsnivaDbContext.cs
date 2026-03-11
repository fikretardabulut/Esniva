using Esniva.Domain.Common;
using Esniva.Domain.Entities;
using Esniva.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Esniva.Infrastructure.Persistence;

public class EsnivaDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    public EsnivaDbContext(
        DbContextOptions<EsnivaDbContext> options,
        ITenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Subdomain)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Price).HasPrecision(18, 2);
        });

        // Tenant query filter
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(CreateTenantFilterExpression(entityType.ClrType));
            }
        }
    }

    private LambdaExpression CreateTenantFilterExpression(Type entityType)
    {
        var method = typeof(EsnivaDbContext)
            .GetMethod(nameof(GetTenantFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .MakeGenericMethod(entityType);

        return (LambdaExpression)method.Invoke(this, null)!;
    }

    private LambdaExpression GetTenantFilter<TEntity>()
        where TEntity : class, ITenantEntity
    {
        return (System.Linq.Expressions.Expression<Func<TEntity, bool>>)
            (e => e.TenantId == _tenantProvider.CurrentTenant!.TenantId);
    }
}