using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Tasks.Queries;

public record GetTasksByQueryParametersQuery(
    Guid ProjectId,
    TaskGetParameters Parameters
): IQuery<PagedResult<TaskDto>>;