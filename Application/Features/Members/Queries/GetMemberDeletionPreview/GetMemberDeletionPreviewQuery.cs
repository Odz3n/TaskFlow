using TaskFlow.Application.DTOs.Project;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Members.Queries;

public record GetMemberDeletionPreviewQuery(
    Guid? ProjectId,
    Guid? MemberId
): IQuery<MemberDeletionPreview>;