using BuildingBlocks.Results;
using Ticketing.Application.DTOs;

namespace Ticketing.Application.Abstractions;

/// <summary>
/// Application-layer abstraction over the real Grpc.Net.Client-based
/// IssueIntake client (implemented in Ticketing.Infrastructure/Grpc), so the
/// Application layer never references generated gRPC stubs directly
/// (AGENTS.md section 12: prefer explicit DTOs across layer boundaries).
/// </summary>
public interface IIssueIntakeClient
{
    Task<Result<CreateTicketResult>> CreateIssueAsync(CreateTicketRequest request, CancellationToken ct = default);
}

/// <summary>Public read contract for ticket history (AGENTS.md section 3).</summary>
public interface ITicketQueries
{
    Task<IReadOnlyList<TicketHistoryItemDto>> ListRecentAsync(int take = 20, CancellationToken ct = default);
}
