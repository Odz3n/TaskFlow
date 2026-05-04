using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Services;

namespace TaskFlow.Application.Features.Tasks.Commands;

public class PartialUpdateTaskCommandHandler
    : ICommandHandler<PartialUpdateTaskCommand, TaskUpdateResponse>
{
    private readonly IProjectPermissionService _permissionService;
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;
    public PartialUpdateTaskCommandHandler(
        IProjectPermissionService permissionService,
        IProjectRepository projectRepository,
        ITaskRepository taskRepository
    )
    {
        _permissionService = permissionService;
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
    }
    public async Task<Result<TaskUpdateResponse>> Handle(
        PartialUpdateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetTrackedProjectByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result<TaskUpdateResponse>.Failure(new Error("Project.NotFound", $"Project with ID '{request.ProjectId}' not found"));

        var task = project.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (task == null)
            return Result<TaskUpdateResponse>.Failure(new Error("Task.NotFound", $"Task with ID '{request.TaskId}' not found"));

        var member = project.Members.FirstOrDefault(m => m.UserId == request.InitiatorId);
        if (member == null)
            return Result<TaskUpdateResponse>.Failure(new Error("Member.NotFound", $"Member '{request.InitiatorId}' is not a member of this project"));

        if (!_permissionService.CanUpdateTask(project, request.InitiatorId, task))
            return Result<TaskUpdateResponse>.Failure(new Error("Member.InsufficientPermissions", "Member cannot update this task"));

        task.Status = request.Status;

        await _taskRepository.UpdateTaskAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return Result<TaskUpdateResponse>.Success(new TaskUpdateResponse(Message: "Task updated successfully"));
    }
}