namespace TaskFlow.Application.DTOs.Responses;

public record RegisterResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? AvatarUrl,
    string Message
);