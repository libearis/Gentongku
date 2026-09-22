using BuildingBlocks.Entities;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities;

/// <summary>
/// Identity module aggregate root. Lives in schema `identity.users`.
/// Password is stored as a salted hash only (see PasswordHasher in Infrastructure).
/// Seller-specific profile data (store name) is intentionally NOT stored here —
/// per AGENTS.md section 3 ("no cross-schema foreign keys"), the store profile
/// belongs to the Catalog module (catalog.seller_profiles) and is created via a
/// plain Id reference (UserId) resolved through Catalog's own Application layer.
/// </summary>
public class User : BaseEntity
{
    public string Username { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string DisplayName { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;

    private User() { }

    public static User Create(string username, string email, string passwordHash, string displayName, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required.", nameof(username));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("Display name is required.", nameof(displayName));

        return new User
        {
            Username = username.Trim().ToLowerInvariant(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            DisplayName = displayName.Trim(),
            Role = role
        };
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
