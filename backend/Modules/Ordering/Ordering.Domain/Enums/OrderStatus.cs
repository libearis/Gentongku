namespace Ordering.Domain.Enums;

/// <summary>Buyer order lifecycle (AGENTS.md section 5).</summary>
public enum OrderStatus
{
    Pending = 0,
    Paid = 1,
    Shipped = 2,
    Completed = 3
}
