using Mapster;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Application.Interfaces.Repositories;

namespace TaskFlow.Application.Features.Tasks.Queries;

public class GetTaskByIdQueryHandler
    : IQueryHandler<GetTaskByIdQuery, TaskDetailDto>
{
    private readonly ITaskRepository _taskRepository;
    public GetTaskByIdQueryHandler(
        ITaskRepository taskRepository
    )
    {
        _taskRepository = taskRepository;
    }
    public async Task<Result<TaskDetailDto>> Handle(
        GetTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetUntrackedTaskByIdAsync(request.TaskId, cancellationToken);
        if (task == null)
            return Result<TaskDetailDto>.Failure(new Error("Task.NotFound", $"Task with ID '{request.TaskId}' not found"));

        return Result<TaskDetailDto>.Success(task.Adapt<TaskDetailDto>());
    }
}