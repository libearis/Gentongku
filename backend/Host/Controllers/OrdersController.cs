using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Abstractions;

namespace Gentongku.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderQueries orderQueries) : ControllerBase
{
    [HttpGet("buyer/{buyerId:guid}")]
    public async Task<IActionResult> GetRecentForBuyer(Guid buyerId, CancellationToken ct) =>
        Ok(await orderQueries.ListRecentAsync(buyerId, 20, ct));
}
