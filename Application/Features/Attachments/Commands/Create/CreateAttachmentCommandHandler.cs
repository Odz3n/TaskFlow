using Mapster;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Attachment;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Application.Interfaces.Services;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Models;
using TaskFlow.Domain.Services;

namespace TaskFlow.Application.Features.Attachments.Commands;

public class CreateAttachmentCommandHandler
    : ICommandHandler<CreateAttachmentCommand, AttachmentDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectPermissionService _permissionService;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IFileService _fileService;
    public CreateAttachmentCommandHandler(
        IProjectRepository projectRepository,
        IProjectPermissionService permissionService,
        IAttachmentRepository attachmentRepository,
        IFileService fileService
    )
    {
        _projectRepository = projectRepository;
        _permissionService = permissionService;
        _attachmentRepository = attachmentRepository;
        _fileService = fileService;
    }
    public async Task<Result<AttachmentDto>> Handle(
        CreateAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetTrackedProjectByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result<AttachmentDto>.Failure(new Error("Project.NotFound", $"Project with ID '{request.ProjectId}' not found"));

        var task = project.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (task == null)
            return Result<AttachmentDto>.Failure(new Error("Task.NotFound", $"Task with ID '{request.TaskId}' not found"));

        var member = project.Members.FirstOrDefault(m => m.UserId == request.InitiatorId);
        if (member == null)
            return Result<AttachmentDto>.Failure(new Error("Member.NotFound", $"User '{request.InitiatorId}' is not a member of this project"));

        if (!_permissionService.CanCreateAttachment(project, request.InitiatorId))
            return Result<AttachmentDto>.Failure(new Error("Member.InsufficientPermissions", $"Member with ID '{member.Id}' cannot create attachments"));

        var saveFileResult = await _fileService.SaveFileAsync(
            ContentType.Users,
            request.InitiatorId,
            SubContentType.Attachments,
            request.File,
            cancellationToken);

        if (string.IsNullOrEmpty(saveFileResult))
            return Result<AttachmentDto>.Failure(new Error("File.UploadFailed", "Failed to save file"));

        var attachment = new Attachment
        {
            TaskId = request.TaskId,
            FileName = request.File.FileName,
            FilePath = saveFileResult,
            FileSize = request.File.Length,
            MemberId = member.Id,
            UploadedDate = DateTime.UtcNow
        };

        await _attachmentRepository.AddAttachmentAsync(attachment, cancellationToken);
        await _attachmentRepository.SaveChangesAsync(cancellationToken);

        return Result<AttachmentDto>.Success(attachment.Adapt<AttachmentDto>());
    }
}