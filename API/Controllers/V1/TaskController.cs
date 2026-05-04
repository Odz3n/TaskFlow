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
        Console.WriteLine("\n\n\n");
        
        Console.WriteLine(User.GetUserId());
        
        Console.WriteLine("\n\n\n");

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
        CancellationToken cancellationToken
    )
    {
        var query = new GetTaskByIdQuery(
            TaskId: taskId
        );

        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result.Data);
    }
    [HttpGet]
    public async Task<IActionResult> GetTasksByParameters(
        [FromQuery] TaskGetParameters parameters,
        CancellationToken cancellationToken
    )
    {
        var query = new GetTasksByQueryParametersQuery(Parameters: parameters);
        var result = await _sender.Send(query, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);
        return Ok(result.Data);
    }
}