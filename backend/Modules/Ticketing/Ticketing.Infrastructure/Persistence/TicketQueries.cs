using Microsoft.EntityFrameworkCore;
using Ticketing.Application.Abstractions;
using Ticketing.Application.DTOs;

namespace Ticketing.Infrastructure.Persistence;

public sealed class TicketQueries : ITicketQueries
{
    private readonly TicketingDbContext _db;

    public TicketQueries(TicketingDbContext db) => _db = db;

    public async Task<IReadOnlyList<TicketHistoryItemDto>> ListRecentAsync(int take = 20, CancellationToken ct = default) =>
        await _db.IssueReports.AsNoTracking()
            .OrderByDescending(i => i.CreatedAt)
            .Take(take)
            .Select(i => new TicketHistoryItemDto(i.Id, i.IssueId, i.Title, i.Severity, i.ReportedBy, i.CreatedAt))
            .ToListAsync(ct);
}
