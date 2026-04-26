using TaskFlow.Application.DTOs.Project;
using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.DTOs.User;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs.Member;

public record MemberDto(
    Guid? Id,
    UserDto? User,
    ProjectDto? Project,
    ProjectRole Role,
    DateTime JoinedDate,
    bool IsActive
);