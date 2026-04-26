using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Member;
using TaskFlow.Application.DTOs.Project;
using TaskFlow.Application.DTOs.User;
using TaskFlow.Application.Features.Projects.Commands;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Services;

namespace TaskFlow.Application.Features.Members.Commands;

public class ChangeMemberRoleCommandHandler : ICommandHandler<ChangeMemberRoleCommand, MemberDto>
{
    private readonly IMemberRepository _memberRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectPermissionService _permissionService;
    public ChangeMemberRoleCommandHandler(
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

    public async Task<Result<MemberDto>> Handle(
        ChangeMemberRoleCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetTrackedProjectByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result<MemberDto>.Failure(new Error("Project.NotFound", $"Project with ID '{request.ProjectId}' not found"));

        if (!_permissionService.CanChangeMemberRole(project, request.InitiatorId, request.TargetMemberId, request.NewRole))
            return Result<MemberDto>.Failure(new Error("Member.NoPermissionToChangeRole", "You do not have sufficient rights to perform this operation"));

        var member = await _memberRepository.GetTrackedMemberByIdAsync(request.TargetMemberId, cancellationToken);
        if (member == null)
            return Result<MemberDto>.Failure(new Error("Member.NotFound", $"Member with ID '{request.TargetMemberId}' not found"));

        member.Role = request.NewRole;

        await _memberRepository.SaveChangesAsync(cancellationToken);

        // Mapper Member => MemberDto
        var dto = new MemberDto(
            Id: member.Id,
            User: new UserDto(
                member.User.UserName,
                member.User.AvatarUrl),
            Project: new ProjectDto(
                member.ProjectId,
                member.Project.Name,
                member.Project.Description,
                member.Project.CreatedDate,
                member.Project.IsArchived),
            Role: member.Role,
            JoinedDate: member.JoinedDate,
            IsActive: member.IsActive);

        return Result<MemberDto>.Success(dto);
    }
}