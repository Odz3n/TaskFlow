namespace TaskFlow.Application.DTOs.Project;

public record UpdateProjectRequest(
    string Name,
    string? Description,
    bool IsArchived
);
