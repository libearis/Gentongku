using Identity.Application.Abstractions;
using Identity.Application.DTOs;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Moq;
using Xunit;

namespace Identity.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<ISellerProfileProvisioner> _sellerProfileProvisioner = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_users.Object, _passwordHasher.Object, _tokenService.Object, _sellerProfileProvisioner.Object);
    }

    private static RegisterRequest ValidBuyerRequest(string username = "buyer1", string email = "buyer1@test.com") =>
        new(username, email, "P@ssw0rd", "Buyer One", UserRole.Buyer, null);

    [Fact]
    public async Task RegisterAsync_WithAdminRole_IsRejected()
    {
        var request = new RegisterRequest("admin1", "admin1@test.com", "P@ssw0rd", "Admin One", UserRole.Admin, null);

        var result = await _sut.RegisterAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal("Admin accounts cannot be self-registered.", result.Error);
        _users.Verify(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_SellerWithoutStoreName_IsRejected()
    {
        var request = new RegisterRequest("seller1", "seller1@test.com", "P@ssw0rd", "Seller One", UserRole.Seller, null);

        var result = await _sut.RegisterAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal("Store name is required for Seller registration.", result.Error);
        _users.Verify(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_SellerWithStoreNameOnlyWhitespace_IsRejected()
    {
        var request = new RegisterRequest("seller1", "seller1@test.com", "P@ssw0rd", "Seller One", UserRole.Seller, "   ");

        var result = await _sut.RegisterAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal("Store name is required for Seller registration.", result.Error);
    }

    [Fact]
    public async Task RegisterAsync_WithAlreadyTakenEmail_IsRejected()
    {
        var request = ValidBuyerRequest();
        _users.Setup(u => u.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(User.Create("existing", request.Email, "hash", "Existing User", UserRole.Buyer));

        var result = await _sut.RegisterAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal("Email is already registered.", result.Error);
        _users.Verify(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WithAlreadyTakenUsername_IsRejected()
    {
        var request = ValidBuyerRequest();
        _users.Setup(u => u.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        _users.Setup(u => u.GetByUsernameAsync(request.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(User.Create(request.Username, "other@test.com", "hash", "Other User", UserRole.Buyer));

        var result = await _sut.RegisterAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal("Username is already taken.", result.Error);
        _users.Verify(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ValidBuyer_SucceedsAndReturnsToken()
    {
        var request = ValidBuyerRequest();
        _users.Setup(u => u.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        _users.Setup(u => u.GetByUsernameAsync(request.Username, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        _passwordHasher.Setup(p => p.Hash(request.Password)).Returns("hashed-password");
        _tokenService.Setup(t => t.IssueToken(It.IsAny<User>()))
            .Returns(("jwt-token", DateTimeOffset.UtcNow.AddHours(1)));

        var result = await _sut.RegisterAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("jwt-token", result.Value!.Token);
        Assert.Equal(request.Username, result.Value.Username);
        Assert.Equal(UserRole.Buyer, result.Value.Role);
        _users.Verify(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _users.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _sellerProfileProvisioner.Verify(s => s.ProvisionAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ValidSeller_ProvisionsSellerProfile()
    {
        var request = new RegisterRequest("seller2", "seller2@test.com", "P@ssw0rd", "Seller Two", UserRole.Seller, "Toko Seller Two");
        _users.Setup(u => u.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        _users.Setup(u => u.GetByUsernameAsync(request.Username, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        _passwordHasher.Setup(p => p.Hash(request.Password)).Returns("hashed-password");
        _tokenService.Setup(t => t.IssueToken(It.IsAny<User>()))
            .Returns(("jwt-token", DateTimeOffset.UtcNow.AddHours(1)));

        var result = await _sut.RegisterAsync(request);

        Assert.True(result.IsSuccess);
        _sellerProfileProvisioner.Verify(s => s.ProvisionAsync(result.Value!.UserId, request.StoreName!, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_Fails()
    {
        var request = new LoginRequest("buyer1@test.com", "WrongPassword");
        var user = User.Create("buyer1", "buyer1@test.com", "correct-hash", "Buyer One", UserRole.Buyer);
        _users.Setup(u => u.GetByEmailOrUsernameAsync(request.Identifier, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(p => p.Verify(request.Password, user.PasswordHash)).Returns(false);

        var result = await _sut.LoginAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal("Invalid email/username or password.", result.Error);
    }

    [Fact]
    public async Task LoginAsync_WithUnknownIdentifier_Fails()
    {
        var request = new LoginRequest("unknown@test.com", "P@ssw0rd");
        _users.Setup(u => u.GetByEmailOrUsernameAsync(request.Identifier, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await _sut.LoginAsync(request);

        Assert.True(result.IsFailure);
        Assert.Equal("Invalid email/username or password.", result.Error);
    }

    [Fact]
    public async Task LoginAsync_WithCorrectCredentials_SucceedsAndReturnsToken()
    {
        var request = new LoginRequest("buyer1@test.com", "P@ssw0rd");
        var user = User.Create("buyer1", "buyer1@test.com", "correct-hash", "Buyer One", UserRole.Buyer);
        _users.Setup(u => u.GetByEmailOrUsernameAsync(request.Identifier, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(p => p.Verify(request.Password, user.PasswordHash)).Returns(true);
        _tokenService.Setup(t => t.IssueToken(user)).Returns(("jwt-token", DateTimeOffset.UtcNow.AddHours(1)));

        var result = await _sut.LoginAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("jwt-token", result.Value!.Token);
        Assert.Equal(user.Email, result.Value.Email);
    }
}
