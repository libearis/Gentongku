using Catalog.Application.Abstractions;
using Catalog.Application.DTOs;
using Identity.Application.Abstractions;
using Ordering.Application;
using Ordering.Application.Abstractions;
using Ordering.Domain.Entities;
using Ordering.Domain.Enums;

namespace Ordering.Infrastructure.Persistence;

// Reads Products/Buyers only through their Application-layer contracts, falling back to a synthetic buyer id so the run is never a no-op.
public sealed class OrderingDummyDataGenerator(OrderingDbContext db, ICatalogQueries catalogQueries, IUserQueries userQueries) : IOrderingDummyDataGenerator
{
    public async Task<int> GenerateOrdersAsync(int count, CancellationToken ct = default)
    {
        var products = await catalogQueries.ListProductsAsync(500, ct);
        var buyerIds = await userQueries.ListBuyerIdsAsync(500, ct);
        if (buyerIds.Count == 0) buyerIds = [Guid.NewGuid()];

        var sellerGroups = products.GroupBy(p => p.SellerId).Select(g => g.ToList()).ToList();
        if (sellerGroups.Count == 0) return 0;

        var couriers = ExpeditionRates.All.Keys.ToList();
        var created = 0;

        for (var offset = 0; offset < count; offset += BatchSize)
        {
            var batch = Math.Min(BatchSize, count - offset);
            var rows = new List<Order>(batch);
            for (var i = 0; i < batch; i++)
            {
                var sellerProducts = Pick(sellerGroups);
                var pickedProducts = PickRandomProducts(sellerProducts);
                if (pickedProducts.Count == 0) continue;

                var items = pickedProducts
                    .Select(p => new OrderItemSpec(p.Id, p.Name, p.Price, _random.Next(1, 4), Pick(couriers), ExpeditionRates.Resolve(Pick(couriers)) ?? 0m))
                    .ToList();

                var order = Order.Create(Pick(buyerIds), pickedProducts[0].SellerId, Guid.NewGuid(), items, "Pesanan data uji.");
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

    private List<ProductDto> PickRandomProducts(IReadOnlyList<ProductDto> products)
    {
        if (products.Count == 0) return [];
        var pickCount = Math.Min(products.Count, _random.Next(1, 4));
        return Enumerable.Range(0, pickCount).Select(_ => Pick(products)).ToList();
    }

    private T Pick<T>(IReadOnlyList<T> pool) => pool[_random.Next(pool.Count)];
}
