using BuildingBlocks.Results;
using Identity.Application.Abstractions;
using Identity.Application.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Enums;

namespace Identity.Application.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
}

// No self-registration for Admin: RegisterAsync rejects UserRole.Admin outright; Admin accounts come only from IdentitySeeder.
public sealed class AuthService(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ISellerProfileProvisioner? sellerProfileProvisioner = null) : IAuthService
{
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (request.Role == UserRole.Admin)
            return Result<AuthResponse>.Failure("Admin accounts cannot be self-registered.");

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Result<AuthResponse>.Failure("Username, email and password are required.");

        if (request.Role == UserRole.Seller && string.IsNullOrWhiteSpace(request.StoreName))
            return Result<AuthResponse>.Failure("Store name is required for Seller registration.");

        var existingEmail = await users.GetByEmailAsync(request.Email, ct);
        if (existingEmail is not null)
            return Result<AuthResponse>.Failure("Email is already registered.");

        var existingUsername = await users.GetByUsernameAsync(request.Username, ct);
        if (existingUsername is not null)
            return Result<AuthResponse>.Failure("Username is already taken.");

        var hash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Username, request.Email, hash, request.DisplayName, request.Role);

        await users.AddAsync(user, ct);
        await users.SaveChangesAsync(ct);

        if (request.Role == UserRole.Seller && sellerProfileProvisioner is not null)
        {
            await sellerProfileProvisioner.ProvisionAsync(user.Id, request.StoreName!, ct);
        }

        var (token, expiresAt) = tokenService.IssueToken(user);
        return Result<AuthResponse>.Success(new AuthResponse(user.Id, user.Username, user.Email, user.DisplayName, user.Role, token, expiresAt));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Identifier) || string.IsNullOrWhiteSpace(request.Password))
            return Result<AuthResponse>.Failure("Invalid email/username or password.");

        var user = await users.GetByEmailOrUsernameAsync(request.Identifier, ct);
        if (user is null || !user.IsActive)
            return Result<AuthResponse>.Failure("Invalid email/username or password.");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("Invalid email/username or password.");

        var (token, expiresAt) = tokenService.IssueToken(user);
        return Result<AuthResponse>.Success(new AuthResponse(user.Id, user.Username, user.Email, user.DisplayName, user.Role, token, expiresAt));
    }
}
