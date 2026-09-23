namespace Catalog.Application.DTOs;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    Guid CategoryId,
    string CategoryName,
    Guid SellerId,
    string SellerName,
    bool IsActive);

public sealed record CategoryDto(Guid Id, string Name);
