using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

// Idempotent, run via `dotnet run -- migrate` (same pattern as IdentitySeeder); distinct from the Scheduler's on-demand dummy-data generator.
public static class CatalogSeeder
{
    public static readonly Guid SystemSellerId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static async Task SeedAsync(CatalogDbContext db)
    {
        await db.Database.MigrateAsync();

        if (await db.Products.AnyAsync()) return;

        if (!await db.SellerProfiles.AnyAsync(s => s.UserId == SystemSellerId))
        {
            db.SellerProfiles.Add(SellerProfile.Create(SystemSellerId, "System Seller"));
        }

        var categories = new[] { "Peralatan Rumah", "Dapur", "Taman", "Dekorasi" }
            .Select(Category.Create)
            .ToList();
        db.Categories.AddRange(categories);

        Guid CategoryId(string name) => categories.First(c => c.Name == name).Id;

        db.Products.AddRange(
            Product.Create("Gentong Tanah Liat Klasik", 185000m, 25, CategoryId("Peralatan Rumah"), SystemSellerId,
                "Gentong tanah liat tradisional untuk menyimpan air minum agar tetap sejuk secara alami."),
            Product.Create("Toples Keramik Bermotif", 92000m, 40, CategoryId("Dapur"), SystemSellerId,
                "Toples keramik dengan motif batik, cocok untuk menyimpan camilan kering."),
            Product.Create("Kendi Air Minum", 65000m, 30, CategoryId("Peralatan Rumah"), SystemSellerId,
                "Kendi tanah liat klasik, menjaga air tetap dingin tanpa kulkas."),
            Product.Create("Pot Tanaman Gerabah", 45000m, 60, CategoryId("Taman"), SystemSellerId,
                "Pot gerabah berpori, baik untuk drainase akar tanaman hias."),
            Product.Create("Cobek & Ulekan Batu", 78000m, 20, CategoryId("Dapur"), SystemSellerId,
                "Cobek batu andesit asli, permukaan kasar alami untuk hasil bumbu yang halus."),
            Product.Create("Celengan Gerabah", 35000m, 50, CategoryId("Dekorasi"), SystemSellerId,
                "Celengan gerabah bentuk klasik, dicat dengan pewarna alami."));

        await db.SaveChangesAsync();
    }
}
