namespace TaskFlow.Application.DTOs.Auth;

public record TokenPayload(
    Guid Id,
    string Email,
    string UserName,
    List<string> Roles,
    DateTime IssuedAt,
    DateTime ExpiresAt
)
{
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public Dictionary<string, string> CustomClaims {get; set;} = new();

};