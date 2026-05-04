using TaskFlow.Application.Common;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Comments.Commands;

public record DeleteCommentCommand(
    Guid CommentId,
    Guid UserId
) : ICommand<bool>;
