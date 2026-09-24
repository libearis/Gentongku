using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Abstractions;

namespace Gentongku.Api.Controllers;

public sealed record CheckoutRequestBody(IReadOnlyList<CheckoutItemRequest> Items);

[ApiController]
[Authorize]
[Route("api/orders")]
public class OrdersController(IOrderQueries orderQueries, ICheckoutService checkoutService, IOrderManagementService orderManagementService) : ControllerBase
{
    [HttpGet("buyer/me")]
    public async Task<IActionResult> GetMyOrders(CancellationToken ct) =>
        Ok(await orderQueries.ListRecentAsync(CurrentUserId(), 50, ct));

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutRequestBody body, CancellationToken ct)
    {
        var result = await checkoutService.CheckoutAsync(CurrentUserId(), body.Items, ct);
        if (result.IsFailure) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("seller/inbox")]
    public async Task<IActionResult> GetSellerInbox(CancellationToken ct) =>
        Ok(await orderManagementService.ListForSellerAsync(CurrentUserId(), ct));

    [HttpPost("{id:guid}/ship")]
    public async Task<IActionResult> MarkShipped(Guid id, CancellationToken ct)
    {
        var result = await orderManagementService.MarkShippedAsync(id, CurrentUserId(), ct);
        if (result.IsFailure) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("{id:guid}/deliver")]
    public async Task<IActionResult> MarkDelivered(Guid id, CancellationToken ct)
    {
        var result = await orderManagementService.MarkDeliveredAsync(id, CurrentUserId(), ct);
        if (result.IsFailure) return BadRequest(new { error = result.Error });
        return Ok();
    }

    private Guid CurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
