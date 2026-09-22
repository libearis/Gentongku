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

/// <summary>
/// Public cross-module read contract (AGENTS.md section 3: "depends on that
/// module's public IXQueries interface, injected via DI from the Host").
/// Other modules (e.g. Catalog resolving a seller's display name) should
/// depend on this instead of Identity.Domain/Infrastructure directly.
/// </summary>
public interface IUserQueries
{
    Task<Identity.Application.DTOs.UserDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
