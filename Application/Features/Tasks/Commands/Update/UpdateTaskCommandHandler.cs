using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Services;

namespace TaskFlow.Application.Features.Tasks.Commands;

public class UpdateTaskCommandHandler
    : ICommandHandler<UpdateTaskCommand, TaskUpdateResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectPermissionService _permissionService;
    public UpdateTaskCommandHandler(
        IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        IProjectPermissionService permissionService
    )
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _permissionService = permissionService;
    }
    public async Task<Result<TaskUpdateResponse>> Handle(
        UpdateTaskCommand request,
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

        var assigneeMember = project.Members.FirstOrDefault(m => m.Id == request.AssigneeMemberId);
        if (assigneeMember == null)
            return Result<TaskUpdateResponse>.Failure(new Error("Member.NotFound", $"Member '{request.AssigneeMemberId}' is not a member of this project"));

        if (!_permissionService.CanUpdateTask(project, request.InitiatorId, task))
            return Result<TaskUpdateResponse>.Failure(new Error("Member.InsufficientPermissions", "Member cannot update this task"));

        task.Title = request.Title;
        task.Description = request.Description;
        task.AssigneeMemberId = assigneeMember.Id;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.DueDate = request.DueDate;

        await _taskRepository.UpdateTaskAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return Result<TaskUpdateResponse>.Success(new TaskUpdateResponse(Message: "Task updated successfully"));
    }
}