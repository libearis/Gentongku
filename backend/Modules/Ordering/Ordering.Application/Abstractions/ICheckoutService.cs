using BuildingBlocks.Results;
using Ordering.Application.DTOs;

namespace Ordering.Application.Abstractions;

public sealed record CheckoutItemRequest(Guid ProductId, int Quantity, string ExpeditionCourier);

public interface ICheckoutService
{
    Task<Result<IReadOnlyList<OrderDto>>> CheckoutAsync(Guid buyerId, IReadOnlyList<CheckoutItemRequest> items, CancellationToken ct = default);
}
