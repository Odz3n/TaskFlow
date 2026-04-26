
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Members.Commands;

public record RemoveMemberCommand(
    Guid? ProjectId,
    Guid? InitiatorId,
    Guid? TargetMemberId,
    MemberDeletionStrategy Strategy
) : ICommand;