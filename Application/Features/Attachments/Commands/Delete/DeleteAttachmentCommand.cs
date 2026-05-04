using TaskFlow.Application.DTOs.Attachment;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Attachments.Commands;

public record DeleteAttachmentCommand(
    Guid ProjectId,
    Guid TaskId,
    Guid InitiatorId,
    Guid AttachmentId
): ICommand<AttachmentDeleteResponse>;