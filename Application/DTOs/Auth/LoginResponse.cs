namespace TaskFlow.Application.DTOs.Auth;

public record LoginResponse(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAt
);
