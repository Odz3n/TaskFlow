namespace TaskFlow.Application.DTOs.Requests;

public record UpdateProjectRequest(
    string Name,
    string? Description,
    bool IsArchived
);
