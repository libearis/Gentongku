using Identity.Domain.Entities;

namespace Identity.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<User?> GetByEmailOrUsernameAsync(string identifier, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

// Public cross-module read contract — other modules (e.g. Catalog resolving a seller's display name) depend on this instead of Identity.Domain/Infrastructure directly.
public interface IUserQueries
{
    Task<Identity.Application.DTOs.UserDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    // Used by the Scheduler module's dummy-data generator to attach realistic BuyerIds to generated Orders without referencing Identity.Domain.
    Task<IReadOnlyList<Guid>> ListBuyerIdsAsync(int take, CancellationToken ct = default);
}
