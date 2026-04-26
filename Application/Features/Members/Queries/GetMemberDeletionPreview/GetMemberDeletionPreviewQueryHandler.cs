using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Project;
using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;

namespace TaskFlow.Application.Features.Members.Queries;

public class GetMemberDeletionPreviewQueryHandler : IQueryHandler<GetMemberDeletionPreviewQuery, MemberDeletionPreview>
{
    private readonly IMemberRepository _memberRepository;
    public GetMemberDeletionPreviewQueryHandler(
        IMemberRepository memberRepository
    )
    {
        _memberRepository = memberRepository;
    }
    public async Task<Result<MemberDeletionPreview>> Handle(
        GetMemberDeletionPreviewQuery request,
        CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetProjectMemberAsync(request.ProjectId, request.MemberId, cancellationToken);
        if (member == null)
            return Result<MemberDeletionPreview>.Failure(new Error("Member.NotFound", $"Member with ID '{request.MemberId}' not found"));

        var preview = new MemberDeletionPreview(
            AssignedTasksCount: member.AssignedTasks.Count(),
            CreatedTasksCount: member.CreatedTasks.Count(),
            CommentsCount: member.Comments.Count(),
            AttachmentsCount: member.Attachments.Count(),
            AssignedTasks: member.AssignedTasks.Select(at => new TaskDto(
                at.Id, at.Title, at.Status, at.DueDate
            )),
            CreatedTasks: member.CreatedTasks.Select(ct => new TaskDto(
                ct.Id, ct.Title, ct.Status, ct.DueDate
            )));

        return Result<MemberDeletionPreview>.Success(preview);
    }
}