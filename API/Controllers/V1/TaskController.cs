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
/// Provides endpoints for managing tasks within a specific project.
/// </summary>
[Authorize]
[ApiVersion(1.0)]
[ApiController]
[Route("api/v{version:apiVersion}/projects/{projectId}/tasks")]
[Produces("application/json")]
public class TaskController : ApiController
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskController"/> class.
    /// </summary>
    /// <param name="sender">Mediator sender used to dispatch commands and queries.</param>
    public TaskController(ISender sender)
        : base(sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a new task within the specified project.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="request">The data required to create a task.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The created task.</returns>
    /// <response code="200">Task was successfully created.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized.</response>
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
    /// Deletes a task by its identifier from the specified project.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The result of the deletion operation.</returns>
    /// <response code="200">Task was successfully deleted.</response>
    /// <response code="400">Deletion failed.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Task not found.</response>
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
    /// Retrieves a specific task by its identifier within a project.
    /// </summary>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The requested task.</returns>
    /// <response code="200">Task was found and returned.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Task not found.</response>
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
    /// Retrieves a paginated and filtered list of tasks for the specified project.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="parameters">Filtering, sorting, and paging parameters.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>A collection of tasks matching the specified criteria.</returns>
    /// <response code="200">Tasks retrieved successfully.</response>
    /// <response code="400">Invalid query parameters.</response>
    /// <response code="401">Unauthorized.</response>
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
    /// Updates all properties of an existing task.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="request">The updated task data.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The updated task.</returns>
    /// <response code="200">Task updated successfully.</response>
    /// <response code="400">Invalid update data.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Task not found.</response>
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
    /// Partially updates a task (for example, changing its status).
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="request">The partial update data.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The updated task.</returns>
    /// <response code="200">Task updated successfully.</response>
    /// <response code="400">Invalid update data.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Task not found.</response>
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