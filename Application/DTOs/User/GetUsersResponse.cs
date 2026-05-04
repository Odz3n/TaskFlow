namespace TaskFlow.Application.DTOs.User;

public record GetUsersResponse(
    Guid Id,
    string FullName,
    string UserName,
    string Email,
    string AvatarUrl
);