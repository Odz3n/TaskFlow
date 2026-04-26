using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.Features.Members.Queries;
using TaskFlow.Domain.Enums;

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
    [HttpGet("{memberId}/deletion-preview")]
    public async Task<IActionResult> GetDeletionPreview(
        Guid projectId,
        Guid memberId,
        CancellationToken cancellationToken
    )
    {
        var query = new GetMemberDeletionPreviewQuery(projectId, memberId);
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
    [HttpDelete("{memberId}")]
    public async Task<IActionResult> RemoveMember(
        Guid projectId,
        Guid memberId,
        [FromQuery] MemberDeletionStrategy strategy,
        CancellationToken cancellationToken
    )
    {
        return Ok();
    }
}