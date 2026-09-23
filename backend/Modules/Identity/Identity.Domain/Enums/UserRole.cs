namespace Identity.Domain.Enums;

// No self-registration for Admin — Admin accounts are seeded, never created through registration.
public enum UserRole
{
    Admin = 0,
    Buyer = 1,
    Seller = 2
}
