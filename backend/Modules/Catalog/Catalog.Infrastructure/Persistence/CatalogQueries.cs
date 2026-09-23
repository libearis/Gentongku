using Catalog.Application.Abstractions;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
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
        await ProductDtoQuery(_db.Products.AsNoTracking().OrderByDescending(p => p.CreatedAt).Take(take))
            .ToListAsync(ct);

    public async Task<ProductDto?> GetProductAsync(Guid id, CancellationToken ct = default) =>
        await ProductDtoQuery(_db.Products.AsNoTracking().Where(p => p.Id == id))
            .FirstOrDefaultAsync(ct);

    private IQueryable<ProductDto> ProductDtoQuery(IQueryable<Product> products) =>
        from p in products
        join c in _db.Categories.AsNoTracking() on p.CategoryId equals c.Id
        join s in _db.SellerProfiles.AsNoTracking() on p.SellerId equals s.UserId into sellerJoin
        from s in sellerJoin.DefaultIfEmpty()
        select new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.StockQuantity,
            p.CategoryId,
            c.Name,
            p.SellerId,
            s.StoreName ?? "Unknown Seller",
            p.IsActive);
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

// Adapts ISellerProfileService to Identity.Application.Abstractions.ISellerProfileProvisioner so Identity never references Catalog directly; only the Host wires the two together.
public sealed class SellerProfileProvisionerAdapter : Identity.Application.Abstractions.ISellerProfileProvisioner
{
    private readonly ISellerProfileService _inner;

    public SellerProfileProvisionerAdapter(ISellerProfileService inner) => _inner = inner;

    public Task ProvisionAsync(Guid userId, string storeName, CancellationToken ct = default) =>
        _inner.ProvisionAsync(userId, storeName, ct);
}
