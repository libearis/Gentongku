using Ordering.Domain.Entities;
using Ordering.Domain.Enums;
using Xunit;

namespace Ordering.Tests;

public class OrderTests
{
    [Fact]
    public void Create_DefaultsStatusToPending()
    {
        var order = Order.Create(Guid.NewGuid(), 150000m);

        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void Create_StoresBuyerIdTotalAmountAndNotes()
    {
        var buyerId = Guid.NewGuid();

        var order = Order.Create(buyerId, 150000m, "Please deliver in the morning");

        Assert.Equal(buyerId, order.BuyerId);
        Assert.Equal(150000m, order.TotalAmount);
        Assert.Equal("Please deliver in the morning", order.Notes);
    }

    [Fact]
    public void Create_WithoutNotes_LeavesNotesNull()
    {
        var order = Order.Create(Guid.NewGuid(), 150000m);

        Assert.Null(order.Notes);
    }

    [Fact]
    public void Create_LeavesUpdatedAtNull()
    {
        var order = Order.Create(Guid.NewGuid(), 150000m);

        Assert.Null(order.UpdatedAt);
    }

    [Fact]
    public void AdvanceStatus_ChangesStatus()
    {
        var order = Order.Create(Guid.NewGuid(), 150000m);

        order.AdvanceStatus(OrderStatus.Paid);

        Assert.Equal(OrderStatus.Paid, order.Status);
    }

    [Fact]
    public void AdvanceStatus_SetsUpdatedAtToNonNull()
    {
        var order = Order.Create(Guid.NewGuid(), 150000m);

        order.AdvanceStatus(OrderStatus.Paid);

        Assert.NotNull(order.UpdatedAt);
    }

    [Fact]
    public void AdvanceStatus_CanProgressThroughFullLifecycle()
    {
        var order = Order.Create(Guid.NewGuid(), 150000m);

        order.AdvanceStatus(OrderStatus.Paid);
        order.AdvanceStatus(OrderStatus.Shipped);
        order.AdvanceStatus(OrderStatus.Completed);

        Assert.Equal(OrderStatus.Completed, order.Status);
    }
}
