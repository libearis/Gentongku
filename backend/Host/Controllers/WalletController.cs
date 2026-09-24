using System.Security.Claims;
using Identity.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gentongku.Api.Controllers;

public sealed record TopUpRequest(decimal Amount);

[ApiController]
[Authorize]
[Route("api/wallet")]
public class WalletController(IWalletService walletService) : ControllerBase
{
    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance(CancellationToken ct)
    {
        var balance = await walletService.GetBalanceAsync(CurrentUserId(), ct);
        return Ok(new { balance });
    }

    [HttpPost("topup")]
    public async Task<IActionResult> TopUp(TopUpRequest request, CancellationToken ct)
    {
        if (request.Amount <= 0) return BadRequest(new { error = "Jumlah top up harus lebih dari 0." });

        await walletService.CreditAsync(CurrentUserId(), request.Amount, ct);
        var balance = await walletService.GetBalanceAsync(CurrentUserId(), ct);
        return Ok(new { balance });
    }

    private Guid CurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
