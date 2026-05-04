namespace TaskFlow.Application.DTOs.Attachment;

public record AttachmentUpdateRequest(
    IFormFile File
);