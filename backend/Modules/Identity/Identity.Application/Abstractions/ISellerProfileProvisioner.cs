namespace Identity.Application.Abstractions;

// Implemented by Catalog.Application and injected from the Host composition root, so Identity never references Catalog.Domain/Infrastructure directly.
public interface ISellerProfileProvisioner
{
    Task ProvisionAsync(Guid userId, string storeName, CancellationToken ct = default);
}
