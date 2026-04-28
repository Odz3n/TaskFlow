using System.Security.Claims;
using TaskFlow.Application.DTOs.Auth;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(User user, IList<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token, CancellationToken cancellationToken);
    TokenPayload GetTokenPayload(string token);
    TokenPayload GetTokenPayloadFromPrincipal(ClaimsPrincipal principal);
}