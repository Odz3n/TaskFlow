namespace TaskFlow.Application.DTOs.Attachment;

public record AttachmentDto(
    Guid Id,
    Guid TaskId,
    Guid? MemberId,
    string? FileName,
    string? FilePath,
    int FileSize,
    DateTime UploadedDate
);
