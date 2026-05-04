using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Services;

namespace TaskFlow.Application.Features.Tasks.Commands;

public class DeleteTaskCommandHandler
    : ICommandHandler<DeleteTaskCommand, TaskDeleteDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectPermissionService _permissionService;
    public DeleteTaskCommandHandler(
        IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        IProjectPermissionService permissionService
    )
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _permissionService = permissionService;
    }
    public async Task<Result<TaskDeleteDto>> Handle(
        DeleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetTrackedProjectByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result<TaskDeleteDto>.Failure(new Error("Project.NotFound", $"Project with ID '{request.ProjectId}' not found"));

        var task = project.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (task == null)
            return Result<TaskDeleteDto>.Failure(new Error("Task.NotFound", $"Task with ID '{request.TaskId}' not found"));

        if (!_permissionService.CanDeleteTask(project, request.InitiatorId, task))
            return Result<TaskDeleteDto>.Failure(new Error("User.InsufficientPermissions", $"User cannot delete this task"));

        await _taskRepository.RemoveTaskAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return Result<TaskDeleteDto>.Success(new TaskDeleteDto(Message: "Task deleted successfully"));
    }
}