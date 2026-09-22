namespace Identity.Domain.Enums;

/// <summary>
/// The three roles supported by the app (AGENTS.md section 4).
/// No self-registration for Admin — Admin accounts are seeded/created
/// from inside the Admin -> Users module only.
/// </summary>
public enum UserRole
{
    Admin = 0,
    Buyer = 1,
    Seller = 2
}
