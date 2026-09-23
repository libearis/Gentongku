namespace Identity.Application.Abstractions;

// Kept abstract so the concrete algorithm (BCrypt) stays swappable from Infrastructure without touching Application/Domain.
public interface IPasswordHasher
{
    string Hash(string plainPassword);
    bool Verify(string plainPassword, string passwordHash);
}
