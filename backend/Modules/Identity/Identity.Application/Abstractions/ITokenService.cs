using Identity.Domain.Entities;

namespace Identity.Application.Abstractions;

/// <summary>
/// Issues JWTs carrying the user's id/email/role claims (AGENTS.md section 4).
/// </summary>
public interface ITokenService
{
    (string Token, DateTimeOffset ExpiresAt) IssueToken(User user);
}
