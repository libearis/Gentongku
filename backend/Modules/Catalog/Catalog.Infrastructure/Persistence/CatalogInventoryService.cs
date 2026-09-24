using Catalog.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

public sealed class CatalogInventoryService(CatalogDbContext db) : ICatalogInventoryService
{
    public async Task<bool> TryReserveStockAsync(Guid productId, int quantity, CancellationToken ct = default)
    {
        var affected = await db.Products
            .Where(p => p.Id == productId && p.IsActive && p.StockQuantity >= quantity)
            .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.StockQuantity, p => p.StockQuantity - quantity), ct);

        return affected > 0;
    }

    public async Task ReleaseStockAsync(Guid productId, int quantity, CancellationToken ct = default)
    {
        await db.Products
            .Where(p => p.Id == productId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.StockQuantity, p => p.StockQuantity + quantity), ct);
    }
}
