namespace TaskFlow.Application.DTOs.Requests;

public record CreateProjectRequest(
    string Name,
    string? Description = null,
    List<Guid>? MemberIds = null
);