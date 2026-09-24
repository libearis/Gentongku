using Catalog.Domain.Entities;
using Xunit;

namespace Catalog.Tests;

public class SellerProfileTests
{
    [Fact]
    public void Create_TrimsWhitespaceFromStoreName()
    {
        var userId = Guid.NewGuid();

        var profile = SellerProfile.Create(userId, "  Toko Gentongku  ");

        Assert.Equal("Toko Gentongku", profile.StoreName);
        Assert.Equal(userId, profile.UserId);
    }

    [Fact]
    public void Create_WithoutAddress_LeavesAddressNull()
    {
        var profile = SellerProfile.Create(Guid.NewGuid(), "Toko Gentongku");

        Assert.Null(profile.Address);
    }

    [Fact]
    public void Create_WithAddress_StoresAddress()
    {
        var profile = SellerProfile.Create(Guid.NewGuid(), "Toko Gentongku", "Jl. Merdeka No. 1");

        Assert.Equal("Jl. Merdeka No. 1", profile.Address);
    }
}
