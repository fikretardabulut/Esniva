using Esniva.Domain.Common;

namespace Esniva.Domain.Entities;

public class Product : BaseEntity, ITenantEntity
{
    public string Name { get; set; } = default!;

    public decimal Price { get; set; }

    public Guid TenantId { get; set; }
}