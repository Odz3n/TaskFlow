using TaskFlow.Application.DTOs.Member;
using TaskFlow.Application.DTOs.Task;

namespace TaskFlow.Application.DTOs.Project;

public record ProjectDto(
    Guid? Id,
    string? Name,
    string? Description,
    DateTime CreatedDate,
    bool IsArchived
);