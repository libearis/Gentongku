namespace Identity.Application.Abstractions;

/// <summary>
/// Password hashing abstraction so the concrete algorithm (BCrypt here) stays
/// swappable from Infrastructure without touching Application/Domain.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string plainPassword);
    bool Verify(string plainPassword, string passwordHash);
}
