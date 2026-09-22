using BuildingBlocks.Results;
using Ticketing.Application.Abstractions;
using Ticketing.Application.DTOs;

namespace Ticketing.Application.Services;

public interface ICreateTicketService
{
    Task<Result<CreateTicketResult>> ExecuteAsync(CreateTicketRequest request, CancellationToken ct = default);
}

/// <summary>
/// CreateTicket use case (AGENTS.md section 8 / docs/external-issue-intake.md).
/// Fire-and-forget by design: on success we only store the returned issue_id
/// locally (Ticketing.Infrastructure persists it), there is no polling for
/// status changes. Failure mapping happens inside IIssueIntakeClient's
/// implementation (INVALID_ARGUMENT / INTERNAL -> friendly Result.Failure).
/// </summary>
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
