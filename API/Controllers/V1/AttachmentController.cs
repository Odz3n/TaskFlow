using System.Reflection.Metadata;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.DTOs.Attachment;
using TaskFlow.Application.Extensions;
using TaskFlow.Application.Features.Attachments.Commands;
using TaskFlow.Application.Features.Attachments.Queries;

namespace TaskFlow.API.Controllers.V1;

/// <summary>
/// Provides endpoints for managing task attachments within a project.
/// </summary>
[Authorize]
[ApiVersion(1.0)]
[ApiController]
[Route("api/v{version:apiVersion}/projects/{projectId}/tasks/{taskId}/[controller]")]
[Produces("application/json")]
public class AttachmentController : ApiController
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttachmentController"/> class.
    /// </summary>
    /// <param name="sender">Mediator sender used to dispatch commands and queries.</param>
    public AttachmentController(ISender sender) : base(sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Uploads a new attachment to a specific task.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="request">The attachment upload data (file).</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The created attachment details.</returns>
    /// <response code="200">Attachment uploaded successfully.</response>
    /// <response code="400">Invalid file or request.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(
        Guid projectId,
        Guid taskId,
        [FromForm] CreateAttachmentRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateAttachmentCommand(
            ProjectId: projectId,
            TaskId: taskId,
            File: request.File,
            InitiatorId: User.GetUserId()
        );

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Retrieves an attachment by its file name.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="filename">The name of the file to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The attachment data.</returns>
    /// <response code="200">Attachment retrieved successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Attachment not found.</response>
    [HttpGet("{filename}")]
    public async Task<IActionResult> GetByFilename(
        Guid projectId,
        Guid taskId,
        string filename,
        CancellationToken cancellationToken
    )
    {
        var query = new GetByFilenameQuery(
            ProjectId: projectId,
            TaskId: taskId,
            FileName: filename);

        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Updates an existing attachment by replacing its file.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="attachmentId">The unique identifier of the attachment.</param>
    /// <param name="request">The updated attachment data (file).</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The updated attachment details.</returns>
    /// <response code="200">Attachment updated successfully.</response>
    /// <response code="400">Invalid file or request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Attachment not found.</response>
    [HttpPut("{attachmentId:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateAttachment(
        Guid projectId,
        Guid taskId,
        Guid attachmentId,
        [FromForm] AttachmentUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateAttachmentCommand(
            ProjectId: projectId,
            TaskId: taskId,
            InitiatorId: User.GetUserId(),
            AttachmentId: attachmentId,
            File: request.File);

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Deletes an attachment from a task.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="attachmentId">The unique identifier of the attachment.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The result of the deletion.</returns>
    /// <response code="200">Attachment deleted successfully.</response>
    /// <response code="400">Deletion failed.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Attachment not found.</response>
    [HttpDelete("{attachmentId:guid}")]
    public async Task<IActionResult> DeleteAttachment(
        Guid projectId,
        Guid taskId,
        Guid attachmentId,
        CancellationToken cancellationToken
    )
    {
        var command = new DeleteAttachmentCommand(
            ProjectId: projectId,
            TaskId: taskId,
            InitiatorId: User.GetUserId(),
            AttachmentId: attachmentId);

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Data);
    }
}