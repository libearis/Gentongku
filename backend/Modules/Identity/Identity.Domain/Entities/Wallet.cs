using BuildingBlocks.Entities;

namespace Identity.Domain.Entities;

public class Wallet : BaseEntity
{
    public Guid UserId { get; private set; }
    public decimal Balance { get; private set; }

    private Wallet() { }

    public static Wallet Create(Guid userId) => new() { UserId = userId, Balance = 0m };
}
