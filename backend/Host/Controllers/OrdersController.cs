using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Abstractions;

namespace Gentongku.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderQueries _orderQueries;

    public OrdersController(IOrderQueries orderQueries) => _orderQueries = orderQueries;

    [HttpGet("buyer/{buyerId:guid}")]
    public async Task<IActionResult> GetRecentForBuyer(Guid buyerId, CancellationToken ct) =>
        Ok(await _orderQueries.ListRecentAsync(buyerId, 20, ct));
}
