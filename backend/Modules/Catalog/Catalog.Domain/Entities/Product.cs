using BuildingBlocks.Entities;

namespace Catalog.Domain.Entities;

// "Produk" read-benchmark table (~86k rows, AGENTS.md section 6.1); SellerId is a plain Guid to identity.users with no cross-schema FK (AGENTS.md section 3).
public class Product : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid SellerId { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Product() { }

    public static Product Create(string name, decimal price, int stockQuantity, Guid categoryId, Guid sellerId, string? description = null) => new()
    {
        Name = name.Trim(),
        Description = description,
        Price = price,
        StockQuantity = stockQuantity,
        CategoryId = categoryId,
        SellerId = sellerId
    };
}
