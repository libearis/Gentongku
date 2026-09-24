namespace Identity.Application.Abstractions;

public interface IWalletService
{
    Task ProvisionAsync(Guid userId, CancellationToken ct = default);

    Task<decimal> GetBalanceAsync(Guid userId, CancellationToken ct = default);

    Task CreditAsync(Guid userId, decimal amount, CancellationToken ct = default);

    // Atomic conditional decrement — returns false (no rows touched) instead of racing a read-then-write check.
    Task<bool> TryDebitAsync(Guid userId, decimal amount, CancellationToken ct = default);
}
