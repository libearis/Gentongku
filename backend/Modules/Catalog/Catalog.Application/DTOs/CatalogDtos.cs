namespace Catalog.Application.DTOs;

public sealed record ProductDto(Guid Id, string Name, string? Description, decimal Price, int StockQuantity, Guid CategoryId, Guid SellerId, bool IsActive);

public sealed record CategoryDto(Guid Id, string Name);
