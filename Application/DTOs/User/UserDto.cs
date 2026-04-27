namespace TaskFlow.Application.DTOs.User;

public record UserDto(
    Guid Id,
    string FullName,
    string UserName,
    string AvatarUrl
);