using BuildingBlocks.Entities;

namespace Ordering.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public string ExpeditionCourier { get; private set; } = default!;
    public decimal ExpeditionCost { get; private set; }

    public decimal LineTotal => UnitPrice * Quantity + ExpeditionCost;

    private OrderItem() { }

    internal static OrderItem Create(
        Guid orderId,
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity,
        string expeditionCourier,
        decimal expeditionCost) => new()
    {
        OrderId = orderId,
        ProductId = productId,
        ProductName = productName,
        UnitPrice = unitPrice,
        Quantity = quantity,
        ExpeditionCourier = expeditionCourier,
        ExpeditionCost = expeditionCost
    };
}
