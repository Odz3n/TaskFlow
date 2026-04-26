using TaskFlow.Application.Common;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Models;
using TaskFlow.Domain.Services;

namespace TaskFlow.Application.Features.Members.Commands;

public class RemoveMemberCommandHandler : ICommandHandler<RemoveMemberCommand>
{
    private readonly IMemberRepository _memberRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectPermissionService _permissionService;

    public RemoveMemberCommandHandler(
        IMemberRepository memberRepository,
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        IProjectPermissionService permissionService
    )
    {
        _memberRepository = memberRepository;
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _permissionService = permissionService;
    }
    public async Task<Result> Handle(
        RemoveMemberCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetTrackedProjectByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result.Failure(new Error("Project.NotFound", $"Project with ID '{request.ProjectId}' not found"));

        if (!_permissionService.CanRemoveMember(project, request.InitiatorId, request.TargetMemberId))
            return Result.Failure(new Error("Member.NoPermissionToRemove", "You do not have sufficient rights to perform this operation"));

        var member = await _memberRepository.GetTrackedMemberByIdAsync(request.TargetMemberId, cancellationToken);
        if (member == null)
            return Result.Failure(new Error("Member.NotFound", $"Member with ID '{request.TargetMemberId}' not found"));

        if (member.Role == ProjectRole.Owner)
            return Result.Failure(new Error("Member.NoPermissionToRemove", $"You do not have sufficient rights to perform this operation on project's owner"));

        await HandleTasksAsync(member, request.Strategy, project, cancellationToken);
        await ClearRelatedDataAsync(member, cancellationToken);

        await _memberRepository.RemoveMemberAsync(member, cancellationToken);
        await _memberRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
    private async System.Threading.Tasks.Task HandleTasksAsync(
        Member member,
        MemberDeletionStrategy strategy,
        Project project,
        CancellationToken cancellationToken)
    {
        switch (strategy)
        {
            case MemberDeletionStrategy.ReassignTasks:
                var owner = project.Members
                    .FirstOrDefault(m => m.Role == ProjectRole.Owner && m.IsActive);
                if (owner != null)
                {
                    foreach (var task in member.AssignedTasks)
                        task.AssigneeMemberId = owner.Id;
                    foreach (var task in member.CreatedTasks)
                        task.CreatorMemberId = owner.Id;
                }
                break;
            case MemberDeletionStrategy.DeleteTasks:
                foreach (var task in member.AssignedTasks)
                    await _taskRepository.RemoveTaskAsync(task, cancellationToken);
                foreach (var task in member.CreatedTasks)
                    await _taskRepository.RemoveTaskAsync(task, cancellationToken);
                break;
            case MemberDeletionStrategy.SetNull:
                foreach (var task in member.AssignedTasks)
                    task.AssigneeMemberId = null;
                foreach (var task in member.CreatedTasks)
                    task.CreatorMemberId = null;
                break;
        }
    }
    private async System.Threading.Tasks.Task ClearRelatedDataAsync(Member member, CancellationToken cancellationToken)
    {
        foreach(var comment in member.Comments)
            comment.MemberId = null;
        foreach(var attachment in member.Attachments)
            attachment.MemberId = null;
    }
}