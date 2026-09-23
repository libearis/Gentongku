using BuildingBlocks.Entities;
using Ordering.Domain.Enums;

namespace Ordering.Domain.Entities;

// Notes is intentionally left unindexed to demonstrate the "no index -> degrades to live query" behavior (AGENTS.md 6.1).
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
