using Catalog.Application.Abstractions;
using Identity.Application.Abstractions;
using Ordering.Application.Abstractions;
using Ordering.Domain.Entities;
using Ordering.Domain.Enums;

namespace Ordering.Infrastructure.Persistence;

// Reads Products/Buyers only through their Application-layer contracts (AGENTS.md 3), falling back to a synthetic buyer id so the run is never a no-op.
public sealed class OrderingDummyDataGenerator(OrderingDbContext db, ICatalogQueries catalogQueries, IUserQueries userQueries) : IOrderingDummyDataGenerator
{
    public async Task<int> GenerateOrdersAsync(int count, CancellationToken ct = default)
    {
        var products = await catalogQueries.ListProductsAsync(500, ct);
        var buyerIds = await userQueries.ListBuyerIdsAsync(500, ct);
        if (buyerIds.Count == 0) buyerIds = [Guid.NewGuid()];

        var created = 0;
        for (var offset = 0; offset < count; offset += BatchSize)
        {
            var batch = Math.Min(BatchSize, count - offset);
            var rows = new List<Order>(batch);
            for (var i = 0; i < batch; i++)
            {
                var pickedProducts = PickRandomProducts(products);
                var total = pickedProducts.Count > 0 ? pickedProducts.Sum(p => p.Price) : _random.Next(20_000, 500_000);
                var notes = pickedProducts.Count > 0
                    ? $"Pesanan: {string.Join(", ", pickedProducts.Select(p => p.Name))}"
                    : "Pesanan data uji (belum ada produk di katalog).";

                var order = Order.Create(Pick(buyerIds), total, notes);
                order.AdvanceStatus(Pick(Statuses));
                rows.Add(order);
            }

            db.Orders.AddRange(rows);
            await db.SaveChangesAsync(ct);
            created += rows.Count;
        }

        return created;
    }

    private const int BatchSize = 500;
    private static readonly OrderStatus[] Statuses = [OrderStatus.Pending, OrderStatus.Paid, OrderStatus.Shipped, OrderStatus.Completed];

    private readonly Random _random = new();

    private List<Catalog.Application.DTOs.ProductDto> PickRandomProducts(IReadOnlyList<Catalog.Application.DTOs.ProductDto> products)
    {
        if (products.Count == 0) return [];
        var pickCount = Math.Min(products.Count, _random.Next(1, 4));
        return Enumerable.Range(0, pickCount).Select(_ => Pick(products)).ToList();
    }

    private T Pick<T>(IReadOnlyList<T> pool) => pool[_random.Next(pool.Count)];
}
