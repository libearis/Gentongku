using Ordering.Domain.Enums;

namespace Ordering.Application.DTOs;

public sealed record OrderDto(Guid Id, Guid BuyerId, decimal TotalAmount, OrderStatus Status, string? Notes, DateTimeOffset CreatedAt);
