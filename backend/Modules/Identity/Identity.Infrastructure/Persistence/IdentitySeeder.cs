using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

/// <summary>
/// Seeds the single Admin account, since Admin has no self-registration path
/// (AGENTS.md section 4). Runs idempotently at Host startup after migrations.
///
/// DEV-ONLY CREDENTIALS (see also docs/dev-seed-credentials.md):
///   username: admin
///   email:    admin@gentongku.local
///   password: Admin#12345 (dev-only, change before any real deployment)
/// </summary>
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
