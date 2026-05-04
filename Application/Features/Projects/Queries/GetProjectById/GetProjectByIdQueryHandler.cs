using Mapster;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Project;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;

namespace TaskFlow.Application.Features.Projects.Queries;

public class GetProjectByIdQueryHandler : IQueryHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectByIdQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<Result<ProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetUntrackedProjectByIdAsync(request.Id, cancellationToken);
        if (project == null)
            return Result<ProjectDto>.Failure(DomainErrors.Project.NotFound);

        return Result<ProjectDto>.Success(project.Adapt<ProjectDto>());
    }
}
