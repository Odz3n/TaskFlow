using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Tasks.Queries;

public record GetTaskByIdQuery(
    Guid TaskId
): IQuery<TaskDetailDto>;