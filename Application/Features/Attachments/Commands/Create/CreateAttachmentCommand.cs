using TaskFlow.Application.DTOs.Attachment;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Attachments.Commands;

public record CreateAttachmentCommand(
    Guid ProjectId,
    Guid TaskId,
    IFormFile File,
    Guid InitiatorId
): ICommand<AttachmentDto>;