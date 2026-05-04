using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.DTOs.Comment;
using TaskFlow.Application.Extensions;
using TaskFlow.Application.Features.Comments.Commands;
using TaskFlow.Application.Features.Comments.Queries;

namespace TaskFlow.API.Controllers.V1;

/// <summary>
/// Controller for managing comments on tasks.
/// </summary>
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

    /// <summary>
    /// Retrieves a paged list of comments for a specific task.
    /// </summary>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="parameters">Paging and sorting parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paged list of comments.</returns>
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

    /// <summary>
    /// Adds a new comment to a task, with an optional file attachment.
    /// </summary>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="request">The comment data (text and optional file).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created comment details.</returns>
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

    /// <summary>
    /// Updates an existing comment's text. Only the author can perform this action.
    /// </summary>
    /// <param name="id">The unique identifier of the comment.</param>
    /// <param name="request">The updated comment text.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated comment details.</returns>
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

    /// <summary>
    /// Deletes a comment. Authorized for the author, project owner, or admin.
    /// </summary>
    /// <param name="id">The unique identifier of the comment to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
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
