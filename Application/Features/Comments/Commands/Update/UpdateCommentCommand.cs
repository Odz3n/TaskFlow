using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Comment;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Comments.Commands;

public record UpdateCommentCommand(
    Guid CommentId,
    Guid UserId,
    string Text
) : ICommand<CommentDto>;
