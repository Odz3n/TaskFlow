using TaskFlow.Application.DTOs.Attachment;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Attachments.Commands;

public record UpdateAttachmentCommand(
    Guid ProjectId,
    Guid TaskId,
    Guid InitiatorId,
    Guid AttachmentId,
    IFormFile File
): ICommand<AttachmentUpdateResponse>;