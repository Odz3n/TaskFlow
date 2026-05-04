using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Attachment;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Application.Interfaces.Services;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Services;

namespace TaskFlow.Application.Features.Attachments.Commands;

public record DeleteAttachmentCommandHandler
    : ICommandHandler<DeleteAttachmentCommand, AttachmentDeleteResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IProjectPermissionService _permissionService;
    private readonly IFileService _fileService;
    public DeleteAttachmentCommandHandler(
        IProjectRepository projectRepository,
        IAttachmentRepository attachmentRepository,
        IProjectPermissionService permissionService,
        IFileService fileService
    )
    {
        _projectRepository = projectRepository;
        _attachmentRepository = attachmentRepository;
        _permissionService = permissionService;
        _fileService = fileService;
    }
    public async Task<Result<AttachmentDeleteResponse>> Handle(
        DeleteAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetTrackedProjectByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result<AttachmentDeleteResponse>.Failure(new Error("Project.NotFound", $"Project with ID '{request.ProjectId}' not found"));

        var task = project.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (task == null)
            return Result<AttachmentDeleteResponse>.Failure(new Error("Task.NotFound", $"Task with ID '{request.TaskId}' not found"));

        var attachment = task.Attachments.FirstOrDefault(a => a.Id == request.AttachmentId);
        if (attachment == null)
            return Result<AttachmentDeleteResponse>.Failure(new Error("Attachment.NotFound", $"Attachment with ID '{request.AttachmentId}' not found"));

        if (!_permissionService.CanDeleteAttachment(project, request.InitiatorId, attachment))
            return Result<AttachmentDeleteResponse>.Failure(new Error("Member.InsufficientPermissions", "Member cannot delete this attachment"));

        var deleteFileResult = await _fileService.DeleteFile(attachment.FilePath!, cancellationToken);
        if (!deleteFileResult)
            return Result<AttachmentDeleteResponse>.Failure(new Error("FileService.FailedToDelete", $"Errors while deleting file '{attachment.FilePath}'"));

        await _attachmentRepository.RemoveAttachmentAsync(attachment, cancellationToken);
        await _attachmentRepository.SaveChangesAsync(cancellationToken);

        return Result<AttachmentDeleteResponse>.Success(new AttachmentDeleteResponse(Message: "Attachment deleted successfully"));
    }
}