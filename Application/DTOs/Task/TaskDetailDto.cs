using TaskFlow.Application.DTOs.Attachment;
using TaskFlow.Application.DTOs.Comment;
using TaskFlow.Application.DTOs.Member;
using TaskFlow.Application.DTOs.Project;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs.Task;

public record TaskDetailDto
(
    Guid Id,
    string Title,
    string? Description,
    ProjectDto Project,
    MemberDto? AssigneeMember,
    MemberDto? CreatorMember,
    Status Status,
    Priority Priority,
    DateTime? DueDate,
    DateTime CreatedDate,
    DateTime? UpdatedDate,
    IEnumerable<CommentDto> Comments,
    IEnumerable<AttachmentDto> Attachments
);