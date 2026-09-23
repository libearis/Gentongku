using Catalog.Application.DTOs;

namespace Catalog.Application.Abstractions;

// Exposed to other modules' Application layers via DI so they never reference Catalog directly (AGENTS.md section 3).
public interface ICatalogQueries
{
    Task<IReadOnlyList<CategoryDto>> ListCategoriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ProductDto>> ListProductsAsync(int take = 20, CancellationToken ct = default);
    Task<ProductDto?> GetProductAsync(Guid id, CancellationToken ct = default);
}

// Registered against Identity.Application.Abstractions.ISellerProfileProvisioner from the Host, so Identity can provision a seller profile without referencing Catalog directly.
public interface ISellerProfileService
{
    Task ProvisionAsync(Guid userId, string storeName, CancellationToken ct = default);
}
