using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.DTOs.Attachment;
using TaskFlow.Application.Extensions;
using TaskFlow.Application.Features.Attachments.Commands;

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
}