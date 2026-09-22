using BuildingBlocks.Entities;

namespace Catalog.Domain.Entities;

/// <summary>
/// Seller's store profile (name, address, category tags). Keyed by the
/// Identity module's UserId as a plain reference, per AGENTS.md section 3
/// ("cross-module references are stored as plain IDs"). This is where the
/// Seller registration's store name (AGENTS.md section 4) is persisted —
/// documented choice: Catalog owns storefront-facing seller data, Identity
/// stays purely about auth/roles.
/// </summary>
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
