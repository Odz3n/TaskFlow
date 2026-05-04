using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Comment;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Comments.Queries;

public record GetCommentsQuery(
    Guid TaskId,
    CommentGetParameters Parameters
) : IQuery<PagedResult<CommentDto>>;
