using TaskFlow.Application.DTOs.Member;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Projects.Commands;

public record ChangeMemberRoleCommand(
    Guid? ProjectId,
    Guid? InitiatorId,
    Guid? TargetMemberId,
    ProjectRole NewRole
): ICommand<MemberDto>;