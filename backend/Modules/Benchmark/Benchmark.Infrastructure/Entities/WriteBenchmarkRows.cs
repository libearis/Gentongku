namespace Benchmark.Infrastructure.Entities;

/// <summary>
/// The two structurally identical write-benchmark tables (AGENTS.md section 6.2):
/// `orders_indexed` (many indexes) vs `orders_plain` (zero indexes). No shared
/// base entity on purpose — kept as plain rows so the write-path timing isn't
/// skewed by unrelated abstractions.
/// </summary>
public class OrderIndexedRow
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BuyerId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class OrderPlainRow
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BuyerId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
