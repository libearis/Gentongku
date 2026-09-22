namespace Identity.Application.Abstractions;

/// <summary>
/// Cross-module contract implemented by the Catalog module's Application layer
/// and injected here from the Host composition root, so Identity can trigger
/// creation of a seller's store profile (catalog.seller_profiles) at
/// registration time without Identity referencing Catalog.Domain/Infrastructure
/// directly (AGENTS.md section 3: modules only talk through Application-layer
/// contracts registered in the composition root).
/// </summary>
public interface ISellerProfileProvisioner
{
    Task ProvisionAsync(Guid userId, string storeName, CancellationToken ct = default);
}
