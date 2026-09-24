using Identity.Application.Abstractions;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public sealed class WalletService(IdentityDbContext db) : IWalletService
{
    public async Task ProvisionAsync(Guid userId, CancellationToken ct = default)
    {
        if (await db.Wallets.AnyAsync(w => w.UserId == userId, ct)) return;

        db.Wallets.Add(Wallet.Create(userId));
        await db.SaveChangesAsync(ct);
    }

    public async Task<decimal> GetBalanceAsync(Guid userId, CancellationToken ct = default) =>
        await db.Wallets.AsNoTracking().Where(w => w.UserId == userId).Select(w => w.Balance).FirstOrDefaultAsync(ct);

    public async Task CreditAsync(Guid userId, decimal amount, CancellationToken ct = default)
    {
        await db.Wallets
            .Where(w => w.UserId == userId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(w => w.Balance, w => w.Balance + amount), ct);
    }

    // Single conditional UPDATE, guarded by Balance >= amount in the same statement Postgres
    // evaluates against the row it locks — two concurrent debits can never both succeed against
    // a balance that only covers one of them, without needing an app-level lock or retry loop.
    public async Task<bool> TryDebitAsync(Guid userId, decimal amount, CancellationToken ct = default)
    {
        var affected = await db.Wallets
            .Where(w => w.UserId == userId && w.Balance >= amount)
            .ExecuteUpdateAsync(setters => setters.SetProperty(w => w.Balance, w => w.Balance - amount), ct);

        return affected > 0;
    }
}
