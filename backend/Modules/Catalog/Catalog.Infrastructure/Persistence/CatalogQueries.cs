using Catalog.Application.Abstractions;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public sealed class CatalogQueries(CatalogDbContext db) : ICatalogQueries
{
    public async Task<IReadOnlyList<CategoryDto>> ListCategoriesAsync(CancellationToken ct = default) =>
        await db.Categories.AsNoTracking()
            .Select(c => new CategoryDto(c.Id, c.Name))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ProductDto>> ListProductsAsync(int take = 20, CancellationToken ct = default) =>
        await ProductDtoQuery(db.Products.AsNoTracking().OrderByDescending(p => p.CreatedAt).Take(take))
            .ToListAsync(ct);

    public async Task<ProductDto?> GetProductAsync(Guid id, CancellationToken ct = default) =>
        await ProductDtoQuery(db.Products.AsNoTracking().Where(p => p.Id == id))
            .FirstOrDefaultAsync(ct);

    private IQueryable<ProductDto> ProductDtoQuery(IQueryable<Product> products) =>
        from p in products
        join c in db.Categories.AsNoTracking() on p.CategoryId equals c.Id
        join s in db.SellerProfiles.AsNoTracking() on p.SellerId equals s.UserId into sellerJoin
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

public sealed class SellerProfileService(CatalogDbContext db) : ISellerProfileService
{
    public async Task ProvisionAsync(Guid userId, string storeName, CancellationToken ct = default)
    {
        var exists = await db.SellerProfiles.AnyAsync(s => s.UserId == userId, ct);
        if (exists) return;

        db.SellerProfiles.Add(Domain.Entities.SellerProfile.Create(userId, storeName));
        await db.SaveChangesAsync(ct);
    }
}

// Adapts ISellerProfileService to Identity.Application.Abstractions.ISellerProfileProvisioner so Identity never references Catalog directly; only the Host wires the two together.
public sealed class SellerProfileProvisionerAdapter(ISellerProfileService inner) : Identity.Application.Abstractions.ISellerProfileProvisioner
{
    public Task ProvisionAsync(Guid userId, string storeName, CancellationToken ct = default) =>
        inner.ProvisionAsync(userId, storeName, ct);
}
