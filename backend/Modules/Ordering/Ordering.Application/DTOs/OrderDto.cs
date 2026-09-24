using Ordering.Domain.Enums;

namespace Ordering.Application.DTOs;

public sealed record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, string ExpeditionCourier, decimal ExpeditionCost);

public sealed record OrderDto(
    Guid Id,
    Guid BuyerId,
    Guid SellerId,
    Guid CheckoutGroupId,
    decimal TotalAmount,
    OrderStatus Status,
    string? Notes,
    DateTimeOffset CreatedAt,
    IReadOnlyList<OrderItemDto> Items);
