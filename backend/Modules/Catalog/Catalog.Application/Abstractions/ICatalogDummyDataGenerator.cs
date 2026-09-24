namespace Catalog.Application.Abstractions;

// Lives in Catalog's own Application layer so Scheduler never touches catalog.* tables directly.
public interface ICatalogDummyDataGenerator
{
    Task<int> GenerateCategoriesAsync(int count, CancellationToken ct = default);

    Task<int> GenerateProductsAsync(int count, CancellationToken ct = default);
}
