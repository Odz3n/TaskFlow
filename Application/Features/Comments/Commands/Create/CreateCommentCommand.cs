using Microsoft.AspNetCore.Http;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Comment;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Comments.Commands;

public record CreateCommentCommand(
    Guid TaskId,
    Guid UserId,
    string Text,
    IFormFile? File
) : ICommand<CommentDto>;
