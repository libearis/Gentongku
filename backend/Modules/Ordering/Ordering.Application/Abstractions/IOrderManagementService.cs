using BuildingBlocks.Results;
using Ordering.Application.DTOs;

namespace Ordering.Application.Abstractions;

public interface IOrderManagementService
{
    Task<IReadOnlyList<OrderDto>> ListForSellerAsync(Guid sellerId, CancellationToken ct = default);

    Task<Result> MarkShippedAsync(Guid orderId, Guid sellerId, CancellationToken ct = default);

    Task<Result> MarkDeliveredAsync(Guid orderId, Guid sellerId, CancellationToken ct = default);
}
