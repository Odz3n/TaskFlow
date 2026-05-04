using Mapster;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Project;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Services;

namespace TaskFlow.Application.Features.Projects.Commands;

public class UpdateProjectCommandHandler : ICommandHandler<UpdateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectPermissionService _permissionService;

    public UpdateProjectCommandHandler(
        IProjectRepository projectRepository,
        IProjectPermissionService permissionService)
    {
        _projectRepository = projectRepository;
        _permissionService = permissionService;
    }

    public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetTrackedProjectByIdAsync(request.Id, cancellationToken);
        if (project == null)
            return Result<ProjectDto>.Failure(DomainErrors.Project.NotFound);

        var userRole = _permissionService.GetUserRole(project, request.InitiatorId);
        if (userRole != ProjectRole.Owner && userRole != ProjectRole.Admin)
            return Result<ProjectDto>.Failure(DomainErrors.Project.PermissionDenied);

        project.Name = request.Name;
        project.Description = request.Description;
        project.IsArchived = request.IsArchived;

        await _projectRepository.UpdateProjectAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        return Result<ProjectDto>.Success(project.Adapt<ProjectDto>());
    }
}
