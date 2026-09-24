namespace Catalog.Application.Abstractions;

public interface ICatalogInventoryService
{
    // Atomic conditional decrement — returns false (no rows touched) instead of racing a read-then-write check.
    Task<bool> TryReserveStockAsync(Guid productId, int quantity, CancellationToken ct = default);

    Task ReleaseStockAsync(Guid productId, int quantity, CancellationToken ct = default);
}
