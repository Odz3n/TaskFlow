using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.Features.Members.Queries;
using TaskFlow.Application.Features.Members.Commands;
using TaskFlow.Application.DTOs.Member;

namespace TaskFlow.API.Controllers.V1;

[Authorize]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/projects/{projectId}/members")]
[ApiController]
[Produces("application/json")]
public class ProjectMembersController : ApiController
{
    private readonly ISender _sender;
    public ProjectMembersController(
        ISender sender) : base(sender)
    {
        _sender = sender;
    }
    [HttpGet("{memberId:guid}/deletion-preview")]
    public async Task<IActionResult> GetDeletionPreview(
        [FromQuery]GetMemberDeletionPreviewQuery query,
        CancellationToken cancellationToken
    )
    {
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);
        if (result.Data!.HasTasks)
        {
            return Ok(new
            {
                preview = result.Data,
                availableStrategies = new[]
                {
                    new { strategy = "ReassignTasks", description = "Reassign all tasks to project owner"},
                    new { strategy = "DeleteTasks", description = "Delete all tasks" },
                    new { strategy = "SetNull", description = "Remove member from tasks" }
                }
            });
        }
        return Ok(new { preview = result.Data });
    }
    [HttpDelete("{memberId:guid}")]
    public async Task<IActionResult> RemoveMember(
        RemoveMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetMembers(
        Guid projectId,
        [FromQuery] MemberGetParameters parameters,
        CancellationToken cancellationToken
    )
    {
        var query = new GetMembersQuery(projectId, parameters);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result.Data);
    }
}