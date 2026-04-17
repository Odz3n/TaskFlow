namespace TaskFlow.Application.DTOs.Responses;

public record GetUsersResponse(
    string UserName,
    string AvatarUrl
);