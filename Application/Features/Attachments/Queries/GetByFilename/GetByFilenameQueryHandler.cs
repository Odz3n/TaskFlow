using Mapster;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Attachment;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;

namespace TaskFlow.Application.Features.Attachments.Queries;

public class GetByFilenameQueryHandler
    : IQueryHandler<GetByFilenameQuery, AttachmentDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IAttachmentRepository _attachmentRepository;
    public GetByFilenameQueryHandler(
        IProjectRepository projectRepository,
        IAttachmentRepository attachmentRepository
    )
    {
        _projectRepository = projectRepository;
        _attachmentRepository = attachmentRepository;
    }

    public async Task<Result<AttachmentDto>> Handle(
        GetByFilenameQuery request,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetTrackedProjectByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result<AttachmentDto>.Failure(new Error("Project.NotFound", $"Project with ID '{request.ProjectId}' not found"));

        var task = project.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (task == null)
            return Result<AttachmentDto>.Failure(new Error("Task.NotFound", $"Task with ID '{request.TaskId}' not found"));

        var attachment = task.Attachments.FirstOrDefault(a => (a.FileName != null ? a.FileName.ToLowerInvariant() : "") == request.FileName.ToLowerInvariant());
        if (attachment == null)
            return Result<AttachmentDto>.Failure(new Error("Attachment.NotFound", $"Attachment with NAME '{request.FileName}' not found"));

        return Result<AttachmentDto>.Success(attachment.Adapt<AttachmentDto>());
    }
}