using BuildingBlocks.Results;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Taskflow.Intake.V1;
using Ticketing.Application.Abstractions;
using Ticketing.Application.DTOs;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Persistence;

namespace Ticketing.Infrastructure.Grpc;

public sealed class IssueIntakeOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 50051;
}

/// <summary>
/// Real Grpc.Net.Client-based client for TaskFlow's IssueIntake service
/// (docs/external-issue-intake.md). Plaintext HTTP/2 only — no TLS, no
/// grpc-web/Envoy (that indirection is for browser clients only; this call
/// always originates from the .NET backend, per AGENTS.md section 8).
/// Maps INVALID_ARGUMENT / INTERNAL to friendly Result.Failure messages
/// instead of swallowing them as a generic error.
/// </summary>
public sealed class GrpcIssueIntakeClient : IIssueIntakeClient
{
    private readonly IssueIntakeOptions _options;
    private readonly TicketingDbContext _db;
    private readonly ILogger<GrpcIssueIntakeClient> _logger;

    public GrpcIssueIntakeClient(IConfiguration configuration, TicketingDbContext db, ILogger<GrpcIssueIntakeClient> logger)
    {
        _options = new IssueIntakeOptions();
        configuration.GetSection("IssueIntake").Bind(_options);
        _db = db;
        _logger = logger;
    }

    public async Task<Result<CreateTicketResult>> CreateIssueAsync(CreateTicketRequest request, CancellationToken ct = default)
    {
        // Plaintext HTTP/2 requires this AppContext switch for Grpc.Net.Client
        // when not using TLS (dev/demo setup per docs/external-issue-intake.md section 2).
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        var address = $"http://{_options.Host}:{_options.Port}";

        try
        {
            using var channel = GrpcChannel.ForAddress(address);
            var client = new IssueIntake.IssueIntakeClient(channel);

            var grpcRequest = new CreateIssueRequest
            {
                Title = request.Title,
                Description = request.Description ?? string.Empty,
                Severity = request.Severity ?? string.Empty,
                TriggerType = request.TriggerType ?? string.Empty,
                ReportedBy = request.ReportedBy
            };

            var response = await client.CreateIssueAsync(grpcRequest, deadline: DateTime.UtcNow.AddSeconds(10), cancellationToken: ct);

            _db.IssueReports.Add(new IssueReport
            {
                IssueId = response.IssueId,
                Title = request.Title,
                Severity = string.IsNullOrWhiteSpace(request.Severity) ? "low" : request.Severity,
                ReportedBy = request.ReportedBy
            });
            await _db.SaveChangesAsync(ct);

            return Result<CreateTicketResult>.Success(new CreateTicketResult(response.IssueId, response.Status));
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
        {
            _logger.LogWarning(ex, "IssueIntake.CreateIssue rejected as INVALID_ARGUMENT");
            return Result<CreateTicketResult>.Failure($"The ticket was rejected: {ex.Status.Detail}");
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Internal)
        {
            _logger.LogError(ex, "IssueIntake.CreateIssue failed with INTERNAL");
            return Result<CreateTicketResult>.Failure("The external ticketing service had an internal error. Please try again later.");
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "IssueIntake.CreateIssue failed with unexpected status {StatusCode}", ex.StatusCode);
            return Result<CreateTicketResult>.Failure($"Ticket submission failed ({ex.StatusCode}).");
        }
    }
}
