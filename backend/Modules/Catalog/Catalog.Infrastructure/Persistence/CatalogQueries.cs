using Catalog.Application.Abstractions;
using Catalog.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public sealed class CatalogQueries : ICatalogQueries
{
    private readonly CatalogDbContext _db;

    public CatalogQueries(CatalogDbContext db) => _db = db;

    public async Task<IReadOnlyList<CategoryDto>> ListCategoriesAsync(CancellationToken ct = default) =>
        await _db.Categories.AsNoTracking()
            .Select(c => new CategoryDto(c.Id, c.Name))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ProductDto>> ListProductsAsync(int take = 20, CancellationToken ct = default) =>
        await _db.Products.AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.CategoryId, p.SellerId, p.IsActive))
            .ToListAsync(ct);
}

public sealed class SellerProfileService : ISellerProfileService
{
    private readonly CatalogDbContext _db;

    public SellerProfileService(CatalogDbContext db) => _db = db;

    public async Task ProvisionAsync(Guid userId, string storeName, CancellationToken ct = default)
    {
        var exists = await _db.SellerProfiles.AnyAsync(s => s.UserId == userId, ct);
        if (exists) return;

        _db.SellerProfiles.Add(Domain.Entities.SellerProfile.Create(userId, storeName));
        await _db.SaveChangesAsync(ct);
    }
}

/// <summary>
/// Adapts Catalog's own <see cref="ISellerProfileService"/> to the contract
/// Identity's Application layer depends on
/// (Identity.Application.Abstractions.ISellerProfileProvisioner), so Identity
/// never references Catalog directly — only the Host wires the two together.
/// </summary>
public sealed class SellerProfileProvisionerAdapter : Identity.Application.Abstractions.ISellerProfileProvisioner
{
    private readonly ISellerProfileService _inner;

    public SellerProfileProvisionerAdapter(ISellerProfileService inner) => _inner = inner;

    public Task ProvisionAsync(Guid userId, string storeName, CancellationToken ct = default) =>
        _inner.ProvisionAsync(userId, storeName, ct);
}
