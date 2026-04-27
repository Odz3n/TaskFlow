namespace TaskFlow.Application.DTOs.Comment;

public record CommentDto(
    Guid Id,
    Guid TaskId,
    Guid? MemberId,
    string Text,
    DateTime CreatedDate,
    DateTime? UpdatedDate,
    string? AttachmentFilePath
);
