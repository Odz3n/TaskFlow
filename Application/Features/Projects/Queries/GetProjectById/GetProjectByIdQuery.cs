using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Project;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Projects.Queries;

public record GetProjectByIdQuery(Guid Id) : IQuery<ProjectDto>;
