using Ordering.Application.DTOs;

namespace Ordering.Application.Abstractions;

/// <summary>
/// Public read contract for the Ordering module (AGENTS.md section 3).
/// Full checkout/cart use cases are TODO for a later pass — this scaffolding
/// pass only proves the module is wired end-to-end with a real migrated table.
/// </summary>
public interface IOrderQueries
{
    Task<IReadOnlyList<OrderDto>> ListRecentAsync(Guid buyerId, int take = 20, CancellationToken ct = default);
}
