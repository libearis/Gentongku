using BuildingBlocks.Results;
using Ticketing.Application.Abstractions;
using Ticketing.Application.DTOs;

namespace Ticketing.Application.Services;

public interface ICreateTicketService
{
    Task<Result<CreateTicketResult>> ExecuteAsync(CreateTicketRequest request, CancellationToken ct = default);
}

// Fire-and-forget by design: only the returned issue_id is stored locally, there is no polling for status changes.
public sealed class CreateTicketService : ICreateTicketService
{
    private readonly IIssueIntakeClient _client;

    public CreateTicketService(IIssueIntakeClient client) => _client = client;

    public async Task<Result<CreateTicketResult>> ExecuteAsync(CreateTicketRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return Result<CreateTicketResult>.Failure("Title is required.");

        return await _client.CreateIssueAsync(request, ct);
    }
}
