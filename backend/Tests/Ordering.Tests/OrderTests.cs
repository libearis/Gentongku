using Ordering.Domain.Entities;
using Ordering.Domain.Enums;
using Xunit;

namespace Ordering.Tests;

public class OrderTests
{
    private static readonly OrderItemSpec SampleItem = new(Guid.NewGuid(), "Gentong Tanah Liat", 150_000m, 2, "JNE", 15_000m);

    private static Order CreateOrder(Guid? buyerId = null, Guid? sellerId = null, string? notes = null, IReadOnlyList<OrderItemSpec>? items = null) =>
        Order.Create(buyerId ?? Guid.NewGuid(), sellerId ?? Guid.NewGuid(), Guid.NewGuid(), items ?? [SampleItem], notes);

    [Fact]
    public void Create_DefaultsStatusToPending()
    {
        var order = CreateOrder();

        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void Create_StoresBuyerSellerAndComputesTotalFromItems()
    {
        var buyerId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        var order = CreateOrder(buyerId, sellerId, "Please deliver in the morning", [SampleItem]);

        Assert.Equal(buyerId, order.BuyerId);
        Assert.Equal(sellerId, order.SellerId);
        Assert.Equal(SampleItem.UnitPrice * SampleItem.Quantity + SampleItem.ExpeditionCost, order.TotalAmount);
        Assert.Equal("Please deliver in the morning", order.Notes);
        Assert.Single(order.Items);
    }

    [Fact]
    public void Create_WithoutItems_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => Order.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), []));
    }

    [Fact]
    public void Create_WithoutNotes_LeavesNotesNull()
    {
        var order = CreateOrder();

        Assert.Null(order.Notes);
    }

    [Fact]
    public void Create_LeavesUpdatedAtNull()
    {
        var order = CreateOrder();

        Assert.Null(order.UpdatedAt);
    }

    [Fact]
    public void MarkPaid_FromPending_Succeeds()
    {
        var order = CreateOrder();

        order.MarkPaid();

        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.NotNull(order.UpdatedAt);
    }

    [Fact]
    public void MarkShipped_WithoutBeingPaidFirst_Throws()
    {
        var order = CreateOrder();

        Assert.Throws<InvalidOperationException>(() => order.MarkShipped());
    }

    [Fact]
    public void MarkDelivered_WithoutBeingShippedFirst_Throws()
    {
        var order = CreateOrder();
        order.MarkPaid();

        Assert.Throws<InvalidOperationException>(() => order.MarkDelivered());
    }

    [Fact]
    public void FullLifecycle_PendingToPaidToShippedToCompleted_Succeeds()
    {
        var order = CreateOrder();

        order.MarkPaid();
        order.MarkShipped();
        order.MarkDelivered();

        Assert.Equal(OrderStatus.Completed, order.Status);
    }

    [Fact]
    public void AdvanceStatus_UnguardedJump_ChangesStatusForDemoData()
    {
        var order = CreateOrder();

        order.AdvanceStatus(OrderStatus.Shipped);

        Assert.Equal(OrderStatus.Shipped, order.Status);
    }
}
