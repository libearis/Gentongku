using Ordering.Application.DTOs;

namespace Ordering.Application.Abstractions;

// Full checkout/cart use cases are TODO; this scaffolding pass only proves the module is wired end-to-end.
public interface IOrderQueries
{
    Task<IReadOnlyList<OrderDto>> ListRecentAsync(Guid buyerId, int take = 20, CancellationToken ct = default);
}
