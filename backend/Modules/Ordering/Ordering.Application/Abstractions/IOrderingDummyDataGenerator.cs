namespace Ordering.Application.Abstractions;

// Kept in Ordering's Application layer so Scheduler never touches ordering.* directly.
public interface IOrderingDummyDataGenerator
{
    // Derives totals from real existing Products/Buyers rather than arbitrary values.
    Task<int> GenerateOrdersAsync(int count, CancellationToken ct = default);
}
