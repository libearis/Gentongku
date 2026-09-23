using Identity.Domain.Enums;

namespace Identity.Application.DTOs;

public sealed record RegisterRequest(string Username, string Email, string Password, string DisplayName, UserRole Role, string? StoreName);

// Identifier accepts either an email address or a username.
public sealed record LoginRequest(string Identifier, string Password);

public sealed record AuthResponse(Guid UserId, string Username, string Email, string DisplayName, UserRole Role, string Token, DateTimeOffset ExpiresAt);

public sealed record UserDto(Guid Id, string Username, string Email, string DisplayName, UserRole Role, bool IsActive, DateTimeOffset CreatedAt);
