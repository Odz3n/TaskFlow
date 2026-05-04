using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Project;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Projects.Commands;

public record UpdateProjectCommand(
    Guid Id,
    Guid InitiatorId,
    string Name,
    string? Description,
    bool IsArchived
) : ICommand<ProjectDto>;
