using Identity.Domain.Entities;

namespace Identity.Application.Abstractions;

// Issues a JWT with the user's id/email/role embedded as claims.
public interface ITokenService
{
    (string Token, DateTimeOffset ExpiresAt) IssueToken(User user);
}
