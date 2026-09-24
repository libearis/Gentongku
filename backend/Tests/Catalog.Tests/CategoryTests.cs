using Catalog.Domain.Entities;
using Xunit;

namespace Catalog.Tests;

public class CategoryTests
{
    [Fact]
    public void Create_TrimsWhitespaceFromName()
    {
        var category = Category.Create("  Elektronik  ");

        Assert.Equal("Elektronik", category.Name);
    }

    [Fact]
    public void Create_AssignsNewId()
    {
        var category = Category.Create("Elektronik");

        Assert.NotEqual(Guid.Empty, category.Id);
    }
}
