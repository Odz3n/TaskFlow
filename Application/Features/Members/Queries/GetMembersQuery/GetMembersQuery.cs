using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Member;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Members.Queries;

public record GetMembersQuery(
    Guid ProjectId,
    MemberGetParameters Parameters
): IQuery<PagedResult<MemberDto>>;