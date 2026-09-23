using BuildingBlocks.Entities;

namespace Catalog.Domain.Entities;

// Keyed by Identity's UserId as a plain reference (AGENTS.md section 3); Catalog owns storefront-facing seller data, Identity stays purely auth/roles.
public class SellerProfile : BaseEntity
{
    public Guid UserId { get; private set; }
    public string StoreName { get; private set; } = default!;
    public string? Address { get; private set; }

    private SellerProfile() { }

    public static SellerProfile Create(Guid userId, string storeName, string? address = null) => new()
    {
        UserId = userId,
        StoreName = storeName.Trim(),
        Address = address
    };
}
