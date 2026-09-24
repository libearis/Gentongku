using Catalog.Application.Abstractions;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence;

// Falls back to CatalogSeeder's System Seller/categories when none exist yet, rather than inventing orphaned category/seller ids.
public sealed class CatalogDummyDataGenerator(CatalogDbContext db) : ICatalogDummyDataGenerator
{
    public async Task<int> GenerateCategoriesAsync(int count, CancellationToken ct = default)
    {
        var created = 0;
        for (var offset = 0; offset < count; offset += BatchSize)
        {
            var batch = Math.Min(BatchSize, count - offset);
            var rows = new List<Category>(batch);
            for (var i = 0; i < batch; i++)
            {
                var name = $"{Pick(CategoryNamePool)} {Guid.NewGuid().ToString("N")[..6]}";
                rows.Add(Category.Create(name));
            }

            db.Categories.AddRange(rows);
            await db.SaveChangesAsync(ct);
            created += rows.Count;
        }

        return created;
    }

    public async Task<int> GenerateProductsAsync(int count, CancellationToken ct = default)
    {
        var categoryIds = await db.Categories.AsNoTracking().Select(c => c.Id).ToListAsync(ct);
        if (categoryIds.Count == 0)
        {
            var fallback = Category.Create("Kerajinan Tangan");
            db.Categories.Add(fallback);
            await db.SaveChangesAsync(ct);
            categoryIds = [fallback.Id];
        }

        var sellerIds = await db.SellerProfiles.AsNoTracking().Select(s => s.UserId).ToListAsync(ct);
        if (sellerIds.Count == 0)
        {
            if (!await db.SellerProfiles.AnyAsync(s => s.UserId == CatalogSeeder.SystemSellerId, ct))
            {
                db.SellerProfiles.Add(SellerProfile.Create(CatalogSeeder.SystemSellerId, "System Seller"));
                await db.SaveChangesAsync(ct);
            }
            sellerIds = [CatalogSeeder.SystemSellerId];
        }

        var created = 0;
        for (var offset = 0; offset < count; offset += BatchSize)
        {
            var batch = Math.Min(BatchSize, count - offset);
            var rows = new List<Product>(batch);
            for (var i = 0; i < batch; i++)
            {
                var name = $"{Pick(ProductNouns)} {Pick(ProductAdjectives)}";
                var price = _random.Next(15_000, 250_000);
                var stock = _random.Next(0, 100);
                rows.Add(Product.Create(
                    name,
                    price,
                    stock,
                    Pick(categoryIds),
                    Pick(sellerIds),
                    $"{name} — hasil kerajinan tangan, dibuat otomatis untuk data uji."));
            }

            db.Products.AddRange(rows);
            await db.SaveChangesAsync(ct);
            created += rows.Count;
        }

        return created;
    }

    private const int BatchSize = 500;

    private static readonly string[] CategoryNamePool =
    [
        "Peralatan Rumah", "Dapur", "Taman", "Dekorasi", "Kerajinan Tangan",
        "Perlengkapan Dapur", "Furnitur Gerabah", "Aksesoris Rumah",
    ];

    private static readonly string[] ProductAdjectives =
    [
        "Klasik", "Modern", "Tradisional", "Motif Batik", "Polos", "Ukir",
        "Glasir", "Bakar Kayu", "Edisi Terbatas", "Warna Alami",
    ];

    private static readonly string[] ProductNouns =
    [
        "Gentong Tanah Liat", "Toples Keramik", "Kendi Air Minum", "Pot Tanaman Gerabah",
        "Cobek Batu", "Celengan Gerabah", "Vas Bunga", "Piring Keramik", "Mangkuk Tanah Liat",
        "Guci Hias",
    ];

    private readonly Random _random = new();

    private T Pick<T>(IReadOnlyList<T> pool) => pool[_random.Next(pool.Count)];
}
