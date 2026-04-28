using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Application.DTOs.Auth;
using TaskFlow.Application.Interfaces.Services;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Settings;

namespace TaskFlow.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    public TokenService(
        JwtSettings jwtSettings
    )
    {
        _jwtSettings = jwtSettings;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string GenerateToken(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim("id", user.Id.ToString()),
            new Claim("sub", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, CancellationToken cancellationToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    public TokenPayload GetTokenPayload(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
            throw new ArgumentException("Invalid token");

        var jwtToken = handler.ReadJwtToken(token);

        return GetTokenPayloadFromPrincipal(new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims)));
    }

    public TokenPayload GetTokenPayloadFromPrincipal(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)
            ?? principal.FindFirst("sub")
            ?? principal.FindFirst("id");

        if (userIdClaim == null)
            throw new ArgumentException("User ID not found in token");

        var payload = new TokenPayload(
            Id: Guid.TryParse(userIdClaim.Value, out var userId) ? userId : Guid.Empty,
            Email: principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            UserName: principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
            Roles: principal.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList(),
            IssuedAt: GetIssuedAt(principal),
            ExpiresAt: GetExpiration(principal)
        );

        var customClaims = principal.Claims
            .Where(c => c.Type != ClaimTypes.NameIdentifier &&
                        c.Type != ClaimTypes.Email &&
                        c.Type != ClaimTypes.Name &&
                        c.Type != ClaimTypes.Role &&
                        c.Type != JwtRegisteredClaimNames.Jti &&
                        c.Type != JwtRegisteredClaimNames.Iat &&
                        c.Type != JwtRegisteredClaimNames.Exp &&
                        c.Type != "id" &&
                        c.Type != "sub")
            .ToDictionary(c => c.Type, c => c.Value);

        payload.CustomClaims = customClaims;

        return payload;
    }
    private static DateTime GetIssuedAt(ClaimsPrincipal principal)
    {
        var iatClaim = principal.FindFirst(JwtRegisteredClaimNames.Iat);
        if (iatClaim != null && long.TryParse(iatClaim.Value, out var seconds))
            return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        return DateTime.UtcNow;
    }
    private static DateTime GetExpiration(ClaimsPrincipal principal)
    {
        var expClaim = principal.FindFirst(JwtRegisteredClaimNames.Exp);
        if (expClaim != null && long.TryParse(expClaim.Value, out var seconds))
            return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
        return DateTime.UtcNow.AddHours(1);
    }
}