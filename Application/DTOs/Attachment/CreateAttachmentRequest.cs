namespace TaskFlow.Application.DTOs.Attachment;

public record CreateAttachmentRequest(
    IFormFile File
);