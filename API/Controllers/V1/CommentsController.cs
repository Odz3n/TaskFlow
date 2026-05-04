using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.DTOs.Comment;
using TaskFlow.Application.DTOs.Requests;
using TaskFlow.Application.Extensions;
using TaskFlow.Application.Features.Comments.Commands;
using TaskFlow.Application.Features.Comments.Queries;

namespace TaskFlow.API.Controllers.V1;

[Authorize]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}")]
[ApiController]
[Produces("application/json")]
public class CommentsController : ApiController
{
    private readonly ISender _sender;

    public CommentsController(ISender sender) : base(sender)
    {
        _sender = sender;
    }

    [HttpGet("tasks/{taskId}/comments")]
    public async Task<IActionResult> GetTaskComments(
        Guid taskId,
        [FromQuery] CommentGetParameters parameters,
        CancellationToken ct)
    {
        var query = new GetCommentsQuery(taskId, parameters);
        var result = await _sender.Send(query, ct);

        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Data);
    }

    [HttpPost("tasks/{taskId}/comments")]
    public async Task<IActionResult> CreateComment(
        Guid taskId,
        [FromForm] CreateCommentRequest request,
        CancellationToken ct)
    {
        var command = new CreateCommentCommand(
            TaskId: taskId,
            UserId: User.GetUserId(),
            Text: request.Text,
            File: request.File
        );

        var result = await _sender.Send(command, ct);

        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Data);
    }

    [HttpPut("comments/{id}")]
    public async Task<IActionResult> UpdateComment(
        Guid id,
        [FromBody] UpdateCommentRequest request,
        CancellationToken ct)
    {
        var command = new UpdateCommentCommand(
            CommentId: id,
            UserId: User.GetUserId(),
            Text: request.Text
        );

        var result = await _sender.Send(command, ct);

        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Data);
    }

    [HttpDelete("comments/{id}")]
    public async Task<IActionResult> DeleteComment(
        Guid id,
        CancellationToken ct)
    {
        var command = new DeleteCommentCommand(
            CommentId: id,
            UserId: User.GetUserId()
        );

        var result = await _sender.Send(command, ct);

        if (result.IsFailure)
            return HandleFailure(result);

        return NoContent();
    }
}
