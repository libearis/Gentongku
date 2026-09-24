using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Xunit;

namespace Identity.Tests;

public class UserTests
{
    [Fact]
    public void Create_NormalizesUsernameAndEmailToLowercaseAndTrimsDisplayName()
    {
        var user = User.Create("  BuyerOne  ", "  Buyer.One@Test.COM  ", "hash", "  Buyer One  ", UserRole.Buyer);

        Assert.Equal("buyerone", user.Username);
        Assert.Equal("buyer.one@test.com", user.Email);
        Assert.Equal("Buyer One", user.DisplayName);
    }

    [Fact]
    public void Create_SetsIsActiveTrueByDefault()
    {
        var user = User.Create("buyer1", "buyer1@test.com", "hash", "Buyer One", UserRole.Buyer);

        Assert.True(user.IsActive);
    }

    [Theory]
    [InlineData("", "email@test.com", "hash", "Display")]
    [InlineData("   ", "email@test.com", "hash", "Display")]
    public void Create_WithBlankUsername_Throws(string username, string email, string hash, string displayName)
    {
        Assert.Throws<ArgumentException>(() => User.Create(username, email, hash, displayName, UserRole.Buyer));
    }

    [Fact]
    public void Create_WithBlankEmail_Throws()
    {
        Assert.Throws<ArgumentException>(() => User.Create("username", "  ", "hash", "Display", UserRole.Buyer));
    }

    [Fact]
    public void Create_WithBlankPasswordHash_Throws()
    {
        Assert.Throws<ArgumentException>(() => User.Create("username", "email@test.com", " ", "Display", UserRole.Buyer));
    }

    [Fact]
    public void Create_WithBlankDisplayName_Throws()
    {
        Assert.Throws<ArgumentException>(() => User.Create("username", "email@test.com", "hash", "  ", UserRole.Buyer));
    }

    [Fact]
    public void Deactivate_SetsIsActiveFalseAndTouchesUpdatedAt()
    {
        var user = User.Create("buyer1", "buyer1@test.com", "hash", "Buyer One", UserRole.Buyer);
        Assert.Null(user.UpdatedAt);

        user.Deactivate();

        Assert.False(user.IsActive);
        Assert.NotNull(user.UpdatedAt);
    }
}
