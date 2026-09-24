using Microsoft.EntityFrameworkCore;
using Ticketing.Application.Abstractions;
using Ticketing.Application.DTOs;

namespace Ticketing.Infrastructure.Persistence;

public sealed class TicketQueries(TicketingDbContext db) : ITicketQueries
{
    public async Task<IReadOnlyList<TicketHistoryItemDto>> ListRecentAsync(int take = 20, CancellationToken ct = default) =>
        await db.IssueReports.AsNoTracking()
            .OrderByDescending(i => i.CreatedAt)
            .Take(take)
            .Select(i => new TicketHistoryItemDto(i.Id, i.IssueId, i.Title, i.Severity, i.ReportedBy, i.CreatedAt))
            .ToListAsync(ct);
}
