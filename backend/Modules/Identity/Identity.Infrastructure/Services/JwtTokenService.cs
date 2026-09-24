using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Application.Abstractions;
using Identity.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Infrastructure.Services;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "gentongku";
    public string Audience { get; set; } = "gentongku-client";
    public string Secret { get; set; } = default!;
    public int ExpiryMinutes { get; set; } = 120;
}

public sealed class JwtTokenService : ITokenService
{
    public JwtTokenService(IConfiguration configuration)
    {
        _options = new JwtOptions();
        configuration.GetSection("Jwt").Bind(_options);
        if (string.IsNullOrWhiteSpace(_options.Secret))
        {
            // Dev-only fallback so the API can still start without a configured secret.
            _options.Secret = "gentongku-dev-only-secret-please-change-0123456789";
        }
    }

    public (string Token, DateTimeOffset ExpiresAt) IssueToken(User user)
    {
        var expires = DateTimeOffset.UtcNow.AddMinutes(_options.ExpiryMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    private readonly JwtOptions _options;
}
