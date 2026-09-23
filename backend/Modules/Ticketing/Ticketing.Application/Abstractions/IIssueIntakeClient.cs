using BuildingBlocks.Results;
using Ticketing.Application.DTOs;

namespace Ticketing.Application.Abstractions;

// Keeps the Application layer from referencing generated gRPC stubs directly; Ticketing.Infrastructure/Grpc implements this over Grpc.Net.Client.
public interface IIssueIntakeClient
{
    Task<Result<CreateTicketResult>> CreateIssueAsync(CreateTicketRequest request, CancellationToken ct = default);
}

public interface ITicketQueries
{
    Task<IReadOnlyList<TicketHistoryItemDto>> ListRecentAsync(int take = 20, CancellationToken ct = default);
}
