using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Abstractions;
using Ticketing.Application.DTOs;
using Ticketing.Application.Services;

namespace Gentongku.Api.Controllers;

[ApiController]
[Route("api/ticketing")]
public class TicketingController(ICreateTicketService createTicketService, ITicketQueries ticketQueries) : ControllerBase
{
    [HttpPost("tickets")]
    public async Task<IActionResult> CreateTicket(CreateTicketRequest request, CancellationToken ct)
    {
        var result = await createTicketService.ExecuteAsync(request, ct);
        if (result.IsFailure) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("tickets")]
    public async Task<IActionResult> ListTickets(CancellationToken ct) => Ok(await ticketQueries.ListRecentAsync(20, ct));
}
