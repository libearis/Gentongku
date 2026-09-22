using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.Abstractions;
using Ticketing.Application.DTOs;
using Ticketing.Application.Services;

namespace Gentongku.Api.Controllers;

[ApiController]
[Route("api/ticketing")]
public class TicketingController : ControllerBase
{
    private readonly ICreateTicketService _createTicketService;
    private readonly ITicketQueries _ticketQueries;

    public TicketingController(ICreateTicketService createTicketService, ITicketQueries ticketQueries)
    {
        _createTicketService = createTicketService;
        _ticketQueries = ticketQueries;
    }

    [HttpPost("tickets")]
    public async Task<IActionResult> CreateTicket(CreateTicketRequest request, CancellationToken ct)
    {
        var result = await _createTicketService.ExecuteAsync(request, ct);
        if (result.IsFailure) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("tickets")]
    public async Task<IActionResult> ListTickets(CancellationToken ct) => Ok(await _ticketQueries.ListRecentAsync(20, ct));
}
