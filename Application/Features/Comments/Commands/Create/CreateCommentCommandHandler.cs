using Mapster;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Comment;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Application.Interfaces.Services;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Models;
using TaskFlow.Domain.Services;

namespace TaskFlow.Application.Features.Comments.Commands;

public class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand, CommentDto>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectPermissionService _permissionService;
    private readonly IFileService _fileService;

    public CreateCommentCommandHandler(
        ICommentRepository commentRepository,
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        IProjectPermissionService permissionService,
        IFileService fileService)
    {
        _commentRepository = commentRepository;
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _permissionService = permissionService;
        _fileService = fileService;
    }

    public async Task<Result<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetUntrackedTaskByIdAsync(request.TaskId, cancellationToken);
        if (task == null)
            return Result<CommentDto>.Failure(DomainErrors.Task.NotFound);

        var project = await _projectRepository.GetUntrackedProjectByIdAsync(task.ProjectId, cancellationToken);
        if (project == null)
            return Result<CommentDto>.Failure(DomainErrors.Project.NotFound);

        if (!_permissionService.CanAddComment(project, request.UserId))
            return Result<CommentDto>.Failure(DomainErrors.Project.PermissionDenied);

        var member = project.Members.FirstOrDefault(m => m.UserId == request.UserId);
        if (member == null)
            return Result<CommentDto>.Failure(DomainErrors.Project.PermissionDenied);

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            TaskId = request.TaskId,
            MemberId = member.Id,
            Text = request.Text,
            CreatedDate = DateTime.UtcNow
        };

        if (request.File != null)
        {
            var filePath = await _fileService.SaveFileAsync(
                ContentType.Comments,
                comment.Id,
                SubContentType.Attachments,
                request.File,
                cancellationToken);
            
            comment.AttachmentFilePath = filePath;
        }

        await _commentRepository.AddCommentAsync(comment, cancellationToken);
        await _commentRepository.SaveChangesAsync(cancellationToken);

        return Result<CommentDto>.Success(comment.Adapt<CommentDto>());
    }
}
