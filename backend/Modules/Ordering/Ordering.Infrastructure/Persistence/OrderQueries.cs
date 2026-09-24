using Microsoft.EntityFrameworkCore;
using Ordering.Application.Abstractions;
using Ordering.Application.DTOs;

namespace Ordering.Infrastructure.Persistence;

public sealed class OrderQueries(OrderingDbContext db) : IOrderQueries
{
    public async Task<IReadOnlyList<OrderDto>> ListRecentAsync(Guid buyerId, int take = 20, CancellationToken ct = default) =>
        await db.Orders.AsNoTracking()
            .Where(o => o.BuyerId == buyerId)
            .OrderByDescending(o => o.CreatedAt)
            .Take(take)
            .Select(o => new OrderDto(o.Id, o.BuyerId, o.TotalAmount, o.Status, o.Notes, o.CreatedAt))
            .ToListAsync(ct);
}
