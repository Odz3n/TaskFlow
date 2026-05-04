using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.DTOs.Project;
using TaskFlow.Application.Extensions;
using TaskFlow.Application.Features.Projects.Commands;
using TaskFlow.Application.Features.Projects.Queries;

namespace TaskFlow.API.Controllers.V1;

/// <summary>
/// Controller for managing projects.
/// </summary>
[Authorize]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public class ProjectsController : ApiController
{
    private readonly ISender _sender;

    public ProjectsController(ISender sender)
        : base(sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Retrieves a paged list of projects based on search and sort criteria.
    /// </summary>
    /// <param name="query">The query parameters including paging, search, and sort.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of projects.</returns>
    [HttpGet]
    public async Task<IActionResult> GetProjects(
        [FromQuery] GetProjectsQuery query,
        CancellationToken cancellationToken
    )
    {
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result.Data);
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="request">The project creation request data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created project details.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateProject(
        [FromBody] CreateProjectRequest request,
        CancellationToken ct
    )
    {
        var command = new CreateProjectCommand(
            InitiatorId: User.GetUserId(),
            InitiatorRoles: User.GetUserRoles().ToList(),
            Name: request.Name,
            Description: request.Description,
            MemberIds: request.MemberIds
        );

        var result = await _sender.Send(command, ct);
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific project by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the project.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The project details if found.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(
        Guid id,
        CancellationToken ct
    )
    {
        var query = new GetProjectByIdQuery(id);
        var result = await _sender.Send(query, ct);
        
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result.Data);
    }

    /// <summary>
    /// Updates an existing project's information.
    /// </summary>
    /// <param name="id">The unique identifier of the project to update.</param>
    /// <param name="request">The project update request data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated project details.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken ct
    )
    {
        var command = new UpdateProjectCommand(
            Id: id,
            InitiatorId: User.GetUserId(),
            Name: request.Name,
            Description: request.Description,
            IsArchived: request.IsArchived
        );

        var result = await _sender.Send(command, ct);
        
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result.Data);
    }

    /// <summary>
    /// Deletes a project. Only authorized users (e.g., Owners or Admins) can perform this action.
    /// </summary>
    /// <param name="id">The unique identifier of the project to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(
        Guid id,
        CancellationToken ct
    )
    {
        var command = new DeleteProjectCommand(
            Id: id,
            InitiatorId: User.GetUserId()
        );

        var result = await _sender.Send(command, ct);
        
        if (result.IsFailure)
            return HandleFailure(result);
        return NoContent();
    }
}