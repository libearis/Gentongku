using Catalog.Domain.Entities;
using Xunit;

namespace Catalog.Tests;

public class ProductTests
{
    [Fact]
    public void Create_SetsIsActiveTrueByDefault()
    {
        var product = Product.Create("Kopi Gentong", 25000m, 100, Guid.NewGuid(), Guid.NewGuid());

        Assert.True(product.IsActive);
    }

    [Fact]
    public void Create_StoresAllFieldsCorrectly()
    {
        var categoryId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        var product = Product.Create("  Kopi Gentong  ", 25000m, 100, categoryId, sellerId, "Kopi khas Gentongku");

        Assert.Equal("Kopi Gentong", product.Name);
        Assert.Equal("Kopi khas Gentongku", product.Description);
        Assert.Equal(25000m, product.Price);
        Assert.Equal(100, product.StockQuantity);
        Assert.Equal(categoryId, product.CategoryId);
        Assert.Equal(sellerId, product.SellerId);
    }

    [Fact]
    public void Create_WithoutDescription_LeavesDescriptionNull()
    {
        var product = Product.Create("Kopi Gentong", 25000m, 100, Guid.NewGuid(), Guid.NewGuid());

        Assert.Null(product.Description);
    }
}
