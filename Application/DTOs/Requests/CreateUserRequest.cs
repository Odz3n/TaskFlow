namespace TaskFlow.Application.DTOs.Requests;

public record CreateUserRequest(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string? AvatarUrl = null
);