using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

// Runs idempotently at Host startup after migrations; dev-only admin credentials are documented in docs/dev-seed-credentials.md.
public static class IdentitySeeder
{
    public const string AdminUsername = "admin";
    public const string AdminEmail = "admin@gentongku.local";
    public const string AdminDevOnlyPassword = "Admin#12345";

    public static async Task SeedAsync(IdentityDbContext db)
    {
        await db.Database.MigrateAsync();

        var exists = await db.Users.AnyAsync(u => u.Email == AdminEmail);
        if (exists) return;

        var hasher = new BcryptPasswordHasher();
        var admin = User.Create(AdminUsername, AdminEmail, hasher.Hash(AdminDevOnlyPassword), "Gentongku Admin", UserRole.Admin);

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}
