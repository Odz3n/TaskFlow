namespace TaskFlow.Application.DTOs.User;

public record CreateUserRequest(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string? AvatarUrl = null
);