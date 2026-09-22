using Identity.Application.Abstractions;

namespace Identity.Infrastructure.Services;

/// <summary>
/// Real password hashing via BCrypt.Net-Next (work factor 11). Chosen over a
/// manual PBKDF2 implementation for battle-tested salt handling; documented here
/// per the "document it" instruction for this pass.
/// </summary>
public sealed class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 11;

    public string Hash(string plainPassword) => BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);

    public bool Verify(string plainPassword, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
}
