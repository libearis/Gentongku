using Identity.Application.Abstractions;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public sealed class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _db;

    public UserRepository(IdentityDbContext db) => _db = db;

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower(), ct);

    public Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.Username == username.Trim().ToLower(), ct);

    public Task<User?> GetByEmailOrUsernameAsync(string identifier, CancellationToken ct = default)
    {
        var normalized = identifier.Trim().ToLower();
        return _db.Users.FirstOrDefaultAsync(u => u.Email == normalized || u.Username == normalized, ct);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task AddAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

public sealed class UserQueries : IUserQueries
{
    private readonly IdentityDbContext _db;

    public UserQueries(IdentityDbContext db) => _db = db;

    public async Task<Identity.Application.DTOs.UserDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
        return user is null
            ? null
            : new Identity.Application.DTOs.UserDto(user.Id, user.Username, user.Email, user.DisplayName, user.Role, user.IsActive, user.CreatedAt);
    }

    public async Task<IReadOnlyList<Guid>> ListBuyerIdsAsync(int take, CancellationToken ct = default) =>
        await _db.Users.AsNoTracking()
            .Where(u => u.Role == UserRole.Buyer && u.IsActive)
            .OrderBy(u => u.Id)
            .Select(u => u.Id)
            .Take(take)
            .ToListAsync(ct);
}
