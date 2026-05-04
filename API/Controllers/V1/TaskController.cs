using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.DTOs.Requests;
using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Extensions;
using TaskFlow.Application.Features.Tasks.Commands;
using TaskFlow.Application.Features.Tasks.Queries;

namespace TaskFlow.API.Controllers.V1;

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