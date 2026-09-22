using BuildingBlocks.Entities;
using Ordering.Domain.Enums;

namespace Ordering.Domain.Entities;

/// <summary>
/// The "Order" read-benchmark table (~2.4M rows at scale), AGENTS.md section 6.1.
/// Indexed columns: Status, CreatedAt, BuyerId. `Notes` is intentionally left
/// unindexed to demonstrate the "no index -> degrades to live query" teaching
/// behavior described in AGENTS.md section 6.1.
/// </summary>
public class Order : BaseEntity
{
    public Guid BuyerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? Notes { get; private set; }

    private Order() { }

    public static Order Create(Guid buyerId, decimal totalAmount, string? notes = null) => new()
    {
        BuyerId = buyerId,
        TotalAmount = totalAmount,
        Status = OrderStatus.Pending,
        Notes = notes
    };

    public void AdvanceStatus(OrderStatus next)
    {
        Status = next;
        Touch();
    }
}
