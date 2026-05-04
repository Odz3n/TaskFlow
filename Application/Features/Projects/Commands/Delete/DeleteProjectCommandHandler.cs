using TaskFlow.Application.Common;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Services;

namespace TaskFlow.Application.Features.Projects.Commands;

public class DeleteProjectCommandHandler : ICommandHandler<DeleteProjectCommand, bool>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectPermissionService _permissionService;

    public DeleteProjectCommandHandler(
        IProjectRepository projectRepository,
        IProjectPermissionService permissionService)
    {
        _projectRepository = projectRepository;
        _permissionService = permissionService;
    }

    public async Task<Result<bool>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetTrackedProjectByIdAsync(request.Id, cancellationToken);
        if (project == null)
            return Result<bool>.Failure(DomainErrors.Project.NotFound);

        var userRole = _permissionService.GetUserRole(project, request.InitiatorId);
        if (userRole != ProjectRole.Owner)
            return Result<bool>.Failure(DomainErrors.Project.PermissionDenied);

        await _projectRepository.RemoveProjectAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
