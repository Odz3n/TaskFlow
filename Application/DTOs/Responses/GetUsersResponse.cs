namespace TaskFlow.Application.DTOs.Responses;

public record GetUsersResponse(
    Guid Id,
    string FullName,
    string UserName,
    string Email,
    string AvatarUrl
);