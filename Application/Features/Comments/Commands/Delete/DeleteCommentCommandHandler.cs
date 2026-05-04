using TaskFlow.Application.Common;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Application.Interfaces.Services;
using TaskFlow.Domain.Services;

namespace TaskFlow.Application.Features.Comments.Commands;

public class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand, bool>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectPermissionService _permissionService;
    private readonly IFileService _fileService;

    public DeleteCommentCommandHandler(
        ICommentRepository commentRepository,
        IProjectRepository projectRepository,
        IProjectPermissionService permissionService,
        IFileService fileService)
    {
        _commentRepository = commentRepository;
        _projectRepository = projectRepository;
        _permissionService = permissionService;
        _fileService = fileService;
    }

    public async Task<Result<bool>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetTrackedCommentByIdAsync(request.CommentId, cancellationToken);
        if (comment == null)
            return Result<bool>.Failure(DomainErrors.Comment.NotFound);

        if (comment.Task == null)
            return Result<bool>.Failure(DomainErrors.Task.NotFound);

        var project = await _projectRepository.GetUntrackedProjectByIdAsync(comment.Task.ProjectId, cancellationToken);
        if (project == null)
            return Result<bool>.Failure(DomainErrors.Project.NotFound);

        if (!_permissionService.CanDeleteComment(project, request.UserId, comment))
            return Result<bool>.Failure(DomainErrors.Comment.PermissionDenied);

        if (!string.IsNullOrEmpty(comment.AttachmentFilePath))
        {
            await _fileService.DeleteFile(comment.AttachmentFilePath, cancellationToken);
        }

        await _commentRepository.RemoveCommentAsync(comment, cancellationToken);
        await _commentRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
