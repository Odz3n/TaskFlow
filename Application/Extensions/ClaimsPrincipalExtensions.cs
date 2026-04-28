using System.Security.Claims;
using TaskFlow.Application.DTOs.Auth;
using TaskFlow.Application.Interfaces.Services;

namespace TaskFlow.Application.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)
                          ?? principal.FindFirst("sub")
                          ?? principal.FindFirst("id");

        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User ID not found int token");

        return Guid.TryParse(userIdClaim.Value, out var userId)
            ? userId
            : Guid.Empty;
    }
    public static string GetUserEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }
    public static string GetUserName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
    }
    public static IEnumerable<string> GetUserRoles(this ClaimsPrincipal principal)
    {
        return principal.FindAll(ClaimTypes.Role).Select(r => r.Value);
    }
    public static TokenPayload GetTokenPayload(this ClaimsPrincipal principal, ITokenService tokenService)
    {
        return tokenService.GetTokenPayloadFromPrincipal(principal);
    }
    public static bool IsInRole(this ClaimsPrincipal principal, string role)
    {
        return principal.IsInRole(role);
    }
}