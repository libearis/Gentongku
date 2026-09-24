using Identity.Application.Abstractions;

namespace Identity.Infrastructure.Services;

// Work factor 11, chosen over a manual PBKDF2 implementation for battle-tested salt handling.
public sealed class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string plainPassword) => BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);

    public bool Verify(string plainPassword, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);

    private const int WorkFactor = 11;
}
