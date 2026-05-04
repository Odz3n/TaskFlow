using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.Features.Members.Queries;
using TaskFlow.Application.Features.Members.Commands;
using TaskFlow.Application.DTOs.Member;

namespace TaskFlow.API.Controllers.V1;

/// <summary>
/// Provides endpoints for managing project members.
/// </summary>
[Authorize]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/projects/{projectId}/members")]
[ApiController]
[Produces("application/json")]
public class ProjectMembersController : ApiController
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectMembersController"/> class.
    /// </summary>
    /// <param name="sender">Mediator sender used to dispatch commands and queries.</param>
    public ProjectMembersController(ISender sender) : base(sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Retrieves a preview of member deletion последствия (e.g., assigned tasks).
    /// </summary>
    /// <param name="query">Query containing member and project identifiers.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>
    /// A preview of the deletion result. If the member has assigned tasks,
    /// includes available resolution strategies.
    /// </returns>
    /// <response code="200">Preview retrieved successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Member not found.</response>
    [HttpGet("{memberId:guid}/deletion-preview")]
    public async Task<IActionResult> GetDeletionPreview(
        [FromQuery] GetMemberDeletionPreviewQuery query,
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
                    new { strategy = "ReassignTasks", description = "Reassign all tasks to project owner" },
                    new { strategy = "DeleteTasks", description = "Delete all tasks" },
                    new { strategy = "SetNull", description = "Remove member from tasks" }
                }
            });
        }

        return Ok(new { preview = result.Data });
    }

    /// <summary>
    /// Removes a member from the specified project.
    /// </summary>
    /// <param name="command">Command containing member removal data.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The result of the removal operation.</returns>
    /// <response code="200">Member removed successfully.</response>
    /// <response code="400">Removal failed.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Member not found.</response>
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

    /// <summary>
    /// Retrieves a paginated and filtered list of members for the specified project.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="parameters">Filtering and paging parameters.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>A collection of project members.</returns>
    /// <response code="200">Members retrieved successfully.</response>
    /// <response code="400">Invalid query parameters.</response>
    /// <response code="401">Unauthorized.</response>
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