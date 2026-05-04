using Mapster;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Features.Tasks.Commands;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Models;
using TaskFlow.Domain.Services;

namespace TaskFlow.Application.Features.Tasks;

public class CreateTaskCommandHandler
    : ICommandHandler<CreateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IProjectPermissionService _permissionService;
    public CreateTaskCommandHandler(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        IMemberRepository memberRepository,
        IProjectPermissionService permissionService
    )
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _memberRepository = memberRepository;
        _permissionService = permissionService;
    }

    public async Task<Result<TaskDto>> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetTrackedProjectByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result<TaskDto>.Failure(new Error("Project.NotFound", "Project not found"));

        var member = project.Members.FirstOrDefault(m => m.UserId == request.InitiatorId);
        if (member == null)
            return Result<TaskDto>.Failure(new Error("Member.NotFound", $"Member '{request.InitiatorId}' is not a member of this project"));

        var assigneeMember = project.Members.FirstOrDefault(m => m.Id == request.AssigneeMemberId);
        if (assigneeMember == null)
            return Result<TaskDto>.Failure(new Error("Member.NotFound", $"Member '{request.AssigneeMemberId}' is not a member of this project"));

        if (!_permissionService.CanCreateTask(project, request.InitiatorId))
            return Result<TaskDto>.Failure(new Error("Member.InsufficientPermissions", "User cannot create tasks"));

        var task = new Domain.Models.Task
        {
            Title = request.Title,
            Description = request.Description,
            ProjectId = request.ProjectId,
            AssigneeMemberId = request.AssigneeMemberId,
            CreatorMemberId = member.Id,
            Status = Status.ToDo,
            Priority = request.Priority,
            DueDate = request.DueDate,
            CreatedDate = DateTime.UtcNow,
            Attachments = new List<Attachment>(),
            Comments = new List<Comment>()
        };

        await _taskRepository.AddTaskAsync(task, cancellationToken);
        await _taskRepository.SaveChangesAsync(cancellationToken);

        return Result<TaskDto>.Success(task.Adapt<TaskDto>());
    }
}