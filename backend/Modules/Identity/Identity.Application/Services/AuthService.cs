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

/// <summary>
/// Register/Login use cases. No self-registration for Admin: RegisterAsync
/// rejects UserRole.Admin outright (AGENTS.md section 4) — Admin accounts are
/// created only via the seed data migration (see Identity.Infrastructure/Persistence/IdentitySeeder.cs).
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ISellerProfileProvisioner? _sellerProfileProvisioner;

    public AuthService(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ISellerProfileProvisioner? sellerProfileProvisioner = null)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _sellerProfileProvisioner = sellerProfileProvisioner;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (request.Role == UserRole.Admin)
            return Result<AuthResponse>.Failure("Admin accounts cannot be self-registered.");

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Result<AuthResponse>.Failure("Username, email and password are required.");

        if (request.Role == UserRole.Seller && string.IsNullOrWhiteSpace(request.StoreName))
            return Result<AuthResponse>.Failure("Store name is required for Seller registration.");

        var existingEmail = await _users.GetByEmailAsync(request.Email, ct);
        if (existingEmail is not null)
            return Result<AuthResponse>.Failure("Email is already registered.");

        var existingUsername = await _users.GetByUsernameAsync(request.Username, ct);
        if (existingUsername is not null)
            return Result<AuthResponse>.Failure("Username is already taken.");

        var hash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Username, request.Email, hash, request.DisplayName, request.Role);

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        if (request.Role == UserRole.Seller && _sellerProfileProvisioner is not null)
        {
            await _sellerProfileProvisioner.ProvisionAsync(user.Id, request.StoreName!, ct);
        }

        var (token, expiresAt) = _tokenService.IssueToken(user);
        return Result<AuthResponse>.Success(new AuthResponse(user.Id, user.Username, user.Email, user.DisplayName, user.Role, token, expiresAt));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Identifier) || string.IsNullOrWhiteSpace(request.Password))
            return Result<AuthResponse>.Failure("Invalid email/username or password.");

        var user = await _users.GetByEmailOrUsernameAsync(request.Identifier, ct);
        if (user is null || !user.IsActive)
            return Result<AuthResponse>.Failure("Invalid email/username or password.");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("Invalid email/username or password.");

        var (token, expiresAt) = _tokenService.IssueToken(user);
        return Result<AuthResponse>.Success(new AuthResponse(user.Id, user.Username, user.Email, user.DisplayName, user.Role, token, expiresAt));
    }
}
