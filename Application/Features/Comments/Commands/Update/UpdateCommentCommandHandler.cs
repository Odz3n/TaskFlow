using Mapster;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Comment;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;
using TaskFlow.Domain.Services;

namespace TaskFlow.Application.Features.Comments.Commands;

public class UpdateCommentCommandHandler : ICommandHandler<UpdateCommentCommand, CommentDto>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectPermissionService _permissionService;

    public UpdateCommentCommandHandler(
        ICommentRepository commentRepository,
        IProjectRepository projectRepository,
        IProjectPermissionService permissionService)
    {
        _commentRepository = commentRepository;
        _projectRepository = projectRepository;
        _permissionService = permissionService;
    }

    public async Task<Result<CommentDto>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetTrackedCommentByIdAsync(request.CommentId, cancellationToken);
        if (comment == null)
            return Result<CommentDto>.Failure(DomainErrors.Comment.NotFound);

        if (comment.Task == null)
             return Result<CommentDto>.Failure(DomainErrors.Task.NotFound);

        var project = await _projectRepository.GetUntrackedProjectByIdAsync(comment.Task.ProjectId, cancellationToken);
        if (project == null)
            return Result<CommentDto>.Failure(DomainErrors.Project.NotFound);

        if (!_permissionService.CanUpdateComment(project, request.UserId, comment))
            return Result<CommentDto>.Failure(DomainErrors.Comment.PermissionDenied);

        comment.Text = request.Text;
        comment.UpdatedDate = DateTime.UtcNow;

        await _commentRepository.UpdateCommentAsync(comment, cancellationToken);
        await _commentRepository.SaveChangesAsync(cancellationToken);

        return Result<CommentDto>.Success(comment.Adapt<CommentDto>());
    }
}
