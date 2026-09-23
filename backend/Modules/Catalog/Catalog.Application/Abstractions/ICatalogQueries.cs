using Catalog.Application.DTOs;

namespace Catalog.Application.Abstractions;

/// <summary>
/// Public read contract for the Catalog module, exposed to other modules'
/// Application layers via DI (AGENTS.md section 3). Also backs the trivial
/// passthrough endpoint proving the module is reachable in this scaffolding pass.
/// </summary>
public interface ICatalogQueries
{
    Task<IReadOnlyList<CategoryDto>> ListCategoriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ProductDto>> ListProductsAsync(int take = 20, CancellationToken ct = default);
    Task<ProductDto?> GetProductAsync(Guid id, CancellationToken ct = default);
}

/// <summary>
/// Implements Identity.Application.Abstractions.ISellerProfileProvisioner from
/// the Catalog side, registered against that interface from the Host so
/// Identity can provision a seller's store profile without referencing Catalog
/// directly.
/// </summary>
public interface ISellerProfileService
{
    Task ProvisionAsync(Guid userId, string storeName, CancellationToken ct = default);
}
