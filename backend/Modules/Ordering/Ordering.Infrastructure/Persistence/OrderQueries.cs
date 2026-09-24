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
            .Select(ToDto())
            .ToListAsync(ct);

    internal static System.Linq.Expressions.Expression<Func<Ordering.Domain.Entities.Order, OrderDto>> ToDto() => o =>
        new OrderDto(
            o.Id,
            o.BuyerId,
            o.SellerId,
            o.CheckoutGroupId,
            o.TotalAmount,
            o.Status,
            o.Notes,
            o.CreatedAt,
            o.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.ExpeditionCourier, i.ExpeditionCost)).ToList());
}
