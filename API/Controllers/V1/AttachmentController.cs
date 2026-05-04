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

[Authorize]
[ApiVersion(1.0)]
[ApiController]
[Route("api/v{version:apiVersion}/projects/{projectId}/tasks/{taskId}/[controller]")]
[Produces("application/json")]
public class AttachmentController : ApiController
{
    private readonly ISender _sender;
    public AttachmentController(
        ISender sender
    ) : base(sender)
    {
        _sender = sender;
    }
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