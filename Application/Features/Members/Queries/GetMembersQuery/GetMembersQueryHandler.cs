using System.Linq.Expressions;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Member;
using TaskFlow.Application.Extensions;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Features.Members.Queries;

public class GetMembersQueryHandler
    : IQueryHandler<GetMembersQuery, PagedResult<MemberDto>>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IProjectRepository _projectRepository;
    public GetMembersQueryHandler(
        IMemberRepository memberRepository,
        IProjectRepository projectRepository
    )
    {
        _memberRepository = memberRepository;
        _projectRepository = projectRepository;
    }
    public async Task<Result<PagedResult<MemberDto>>> Handle(
        GetMembersQuery request,
        CancellationToken cancellationToken)
    {
        var projectExists = await _projectRepository.ProjectExistsAsync(request.ProjectId, cancellationToken);
        if (!projectExists)
            return Result<PagedResult<MemberDto>>.Failure(new Error("Project.NotFound", $"Project with ID '{request.ProjectId}' not found"));

        var sortingMap = new Dictionary<string, Expression<Func<Member, object>>>
        {
            ["username"] = m => m.User.UserName??"",
            ["projectrole"] = m => m.Role,
            ["joineddate"] = m => m.JoinedDate,
            ["isactive"] = m => m.IsActive
        };

        var result = await _memberRepository
            .GetMembersQueryable(cancellationToken)
            .Where(m => m.ProjectId == request.ProjectId)
            .ApplyMemberSearch(request.Parameters)
            .ApplySort(request.Parameters, sortingMap)
            .ToPagedResultAsync<Member, MemberDto>(request.Parameters, cancellationToken);

        return Result<PagedResult<MemberDto>>.Success(result);
    }
}