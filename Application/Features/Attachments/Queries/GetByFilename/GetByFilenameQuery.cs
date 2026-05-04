using TaskFlow.Application.DTOs.Attachment;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Attachments.Queries;

public record GetByFilenameQuery(
    Guid ProjectId,
    Guid TaskId,
    string FileName
): IQuery<AttachmentDto>;