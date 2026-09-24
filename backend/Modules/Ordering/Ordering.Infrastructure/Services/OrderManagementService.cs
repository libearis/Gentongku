using BuildingBlocks.Results;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Abstractions;
using Ordering.Application.DTOs;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Services;

public sealed class OrderManagementService(OrderingDbContext db) : IOrderManagementService
{
    public async Task<IReadOnlyList<OrderDto>> ListForSellerAsync(Guid sellerId, CancellationToken ct = default) =>
        await db.Orders.AsNoTracking()
            .Where(o => o.SellerId == sellerId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(OrderQueries.ToDto())
            .ToListAsync(ct);

    public async Task<Result> MarkShippedAsync(Guid orderId, Guid sellerId, CancellationToken ct = default)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == orderId, ct);
        if (order is null) return Result.Failure("Pesanan tidak ditemukan.");
        if (order.SellerId != sellerId) return Result.Failure("Pesanan ini bukan milik toko Anda.");

        try
        {
            order.MarkShipped();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> MarkDeliveredAsync(Guid orderId, Guid sellerId, CancellationToken ct = default)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == orderId, ct);
        if (order is null) return Result.Failure("Pesanan tidak ditemukan.");
        if (order.SellerId != sellerId) return Result.Failure("Pesanan ini bukan milik toko Anda.");

        try
        {
            order.MarkDelivered();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
