using BuildingBlocks.Results;
using Catalog.Application.Abstractions;
using Catalog.Application.DTOs;
using Identity.Application.Abstractions;
using Ordering.Application;
using Ordering.Application.Abstractions;
using Ordering.Application.DTOs;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Services;

public sealed class CheckoutService(
    OrderingDbContext db,
    ICatalogQueries catalogQueries,
    ICatalogInventoryService inventoryService,
    IWalletService walletService) : ICheckoutService
{
    public async Task<Result<IReadOnlyList<OrderDto>>> CheckoutAsync(Guid buyerId, IReadOnlyList<CheckoutItemRequest> items, CancellationToken ct = default)
    {
        if (items.Count == 0)
            return Result<IReadOnlyList<OrderDto>>.Failure("Keranjang kosong.");

        var resolved = new List<(ProductDto Product, CheckoutItemRequest Request, decimal ExpeditionCost)>();
        foreach (var item in items)
        {
            if (item.Quantity <= 0)
                return Result<IReadOnlyList<OrderDto>>.Failure("Jumlah produk harus lebih dari 0.");

            var product = await catalogQueries.GetProductAsync(item.ProductId, ct);
            if (product is null || !product.IsActive)
                return Result<IReadOnlyList<OrderDto>>.Failure("Salah satu produk tidak ditemukan atau sudah tidak aktif.");

            var expeditionCost = ExpeditionRates.Resolve(item.ExpeditionCourier);
            if (expeditionCost is null)
                return Result<IReadOnlyList<OrderDto>>.Failure($"Ekspedisi \"{item.ExpeditionCourier}\" tidak dikenali.");

            resolved.Add((product, item, expeditionCost.Value));
        }

        var grandTotal = resolved.Sum(r => r.Product.Price * r.Request.Quantity + r.ExpeditionCost);

        // Step 1: reserve the money first, atomically — see WalletService.TryDebitAsync.
        if (!await walletService.TryDebitAsync(buyerId, grandTotal, ct))
            return Result<IReadOnlyList<OrderDto>>.Failure("Saldo wallet tidak cukup.");

        // Step 2: reserve stock per line, atomically — see CatalogInventoryService.TryReserveStockAsync.
        // If any line fails partway, compensate everything reserved so far (this is the saga: no
        // single cross-schema transaction spans Identity + Catalog + Ordering, so failure recovery
        // is explicit rollback calls instead of a database ROLLBACK).
        var reserved = new List<(Guid ProductId, int Quantity)>();
        foreach (var (product, request, _) in resolved)
        {
            if (await inventoryService.TryReserveStockAsync(product.Id, request.Quantity, ct))
            {
                reserved.Add((product.Id, request.Quantity));
                continue;
            }

            foreach (var (productId, quantity) in reserved)
                await inventoryService.ReleaseStockAsync(productId, quantity, ct);
            await walletService.CreditAsync(buyerId, grandTotal, ct);

            return Result<IReadOnlyList<OrderDto>>.Failure($"Stok produk \"{product.Name}\" tidak cukup.");
        }

        var checkoutGroupId = Guid.NewGuid();
        var orders = resolved
            .GroupBy(r => r.Product.SellerId)
            .Select(group =>
            {
                var order = Order.Create(
                    buyerId,
                    group.Key,
                    checkoutGroupId,
                    group.Select(r => new OrderItemSpec(r.Product.Id, r.Product.Name, r.Product.Price, r.Request.Quantity, r.Request.ExpeditionCourier, r.ExpeditionCost)).ToList());
                order.MarkPaid();
                return order;
            })
            .ToList();

        db.Orders.AddRange(orders);
        await db.SaveChangesAsync(ct);

        var dtoSelector = OrderQueries.ToDto().Compile();
        return Result<IReadOnlyList<OrderDto>>.Success(orders.Select(dtoSelector).ToList());
    }
}
