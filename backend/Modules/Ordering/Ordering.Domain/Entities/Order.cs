using BuildingBlocks.Entities;
using Ordering.Domain.Enums;

namespace Ordering.Domain.Entities;

public sealed record OrderItemSpec(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, string ExpeditionCourier, decimal ExpeditionCost);

// Notes is intentionally left unindexed to demonstrate the "no index -> degrades to live query" behavior.
public class Order : BaseEntity
{
    private readonly List<OrderItem> _items = new();

    public Guid BuyerId { get; private set; }
    public Guid SellerId { get; private set; }
    public Guid CheckoutGroupId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public static Order Create(Guid buyerId, Guid sellerId, Guid checkoutGroupId, IReadOnlyList<OrderItemSpec> items, string? notes = null)
    {
        if (items.Count == 0) throw new InvalidOperationException("An order must have at least one item.");

        var order = new Order
        {
            BuyerId = buyerId,
            SellerId = sellerId,
            CheckoutGroupId = checkoutGroupId,
            Status = OrderStatus.Pending,
            Notes = notes
        };

        foreach (var item in items)
        {
            order._items.Add(OrderItem.Create(order.Id, item.ProductId, item.ProductName, item.UnitPrice, item.Quantity, item.ExpeditionCourier, item.ExpeditionCost));
        }

        order.TotalAmount = order._items.Sum(i => i.LineTotal);
        return order;
    }

    public void MarkPaid()
    {
        if (Status != OrderStatus.Pending) throw new InvalidOperationException($"Cannot mark paid from status {Status}.");
        Status = OrderStatus.Paid;
        Touch();
    }

    public void MarkShipped()
    {
        if (Status != OrderStatus.Paid) throw new InvalidOperationException($"Cannot mark shipped from status {Status}.");
        Status = OrderStatus.Shipped;
        Touch();
    }

    public void MarkDelivered()
    {
        if (Status != OrderStatus.Shipped) throw new InvalidOperationException($"Cannot mark delivered from status {Status}.");
        Status = OrderStatus.Completed;
        Touch();
    }

    // Unguarded status jump, used only to fabricate varied demo data (see Scheduler's OrderingDummyDataGenerator).
    public void AdvanceStatus(OrderStatus next)
    {
        Status = next;
        Touch();
    }
}
