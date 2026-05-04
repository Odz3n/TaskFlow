namespace TaskFlow.Application.DTOs.Project;

public record CreateProjectRequest(
    string Name,
    string? Description = null,
    List<Guid>? MemberIds = null
);