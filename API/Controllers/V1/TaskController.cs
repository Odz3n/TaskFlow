using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Extensions;
using TaskFlow.Application.Features.Tasks.Commands;
using TaskFlow.Application.Features.Tasks.Queries;

namespace TaskFlow.API.Controllers.V1;

/// <summary>
/// Controller for managing tasks within projects.
/// </summary>
[Authorize]
[ApiVersion(1.0)]
[ApiController]
[Route("api/v{version:apiVersion}/projects/{projectId}/tasks")]
[Produces("application/json")]
public class TaskController : ApiController
{
    private readonly ISender _sender;
    public TaskController(ISender sender)
        : base(sender)
    {
        _sender = sender;
    }
    /// <summary>
    /// Creates a new task in a project.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="request">The task creation request data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created task details.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateTask(
        Guid projectId,
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateTaskCommand(
            Title: request.Title,
            Description: request.Description,
            ProjectId: projectId,
            AssigneeMemberId: request.AssigneeMemberId,
            InitiatorId: User.GetUserId(),
            Priority: request.Priority,
            DueDate: request.DueDate
        );

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result.Data);
    }

    /// <summary>
    /// Deletes a task from a project by its ID.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="taskId">The unique identifier of the task to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the deletion.</returns>
    [HttpDelete("{taskId:Guid}")]
    public async Task<IActionResult> DeleteById(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken
    )
    {
        var command = new DeleteTaskCommand(
            InitiatorId: User.GetUserId(),
            ProjectId: projectId,
            TaskId: taskId
        );

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result.Data);
    }
    /// <summary>
    /// Retrieves a specific task by its ID within a project.
    /// </summary>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task details if found.</returns>
    [HttpGet("{taskId:Guid}")]
    public async Task<IActionResult> GetTaskById(
        Guid taskId,
        Guid projectId,
        CancellationToken cancellationToken
    )
    {
        var query = new GetTaskByIdQuery(
            TaskId: taskId,
            ProjectId: projectId
        );

        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result.Data);
    }
    /// <summary>
    /// Retrieves a paged list of tasks for a project based on query parameters.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="parameters">The search and paging parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of tasks.</returns>
    [HttpGet]
    public async Task<IActionResult> GetTasksByParameters(
        Guid projectId,
        [FromQuery] TaskGetParameters parameters,
        CancellationToken cancellationToken
    )
    {
        var query = new GetTasksByQueryParametersQuery(
            ProjectId: projectId,
            Parameters: parameters);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result.Data);
    }
    /// <summary>
    /// Updates an existing task's information.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="taskId">The unique identifier of the task to update.</param>
    /// <param name="request">The task update data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated task details.</returns>
    [HttpPut("{taskId:guid}")]
    public async Task<IActionResult> Update(
        Guid projectId,
        Guid taskId,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateTaskCommand(
            ProjectId: projectId,
            TaskId: taskId,
            InitiatorId: User.GetUserId(),
            Title: request.Title,
            Description: request.Description,
            AssigneeMemberId: request.AssigneeMemberId,
            Status: request.Status,
            Priority: request.Priority,
            DueDate: request.DueDate);
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result.Data);
    }
    /// <summary>
    /// Partially updates a task (e.g., changes its status).
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="request">The partial update data (e.g., Status).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated task details.</returns>
    [HttpPatch("{taskId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid projectId,
        Guid taskId,
        [FromBody] PartialUpdateTaskRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new PartialUpdateTaskCommand(
            ProjectId: projectId,
            TaskId: taskId,
            InitiatorId: User.GetUserId(),
            Status: request.Status);

        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result.Data);
    }
}