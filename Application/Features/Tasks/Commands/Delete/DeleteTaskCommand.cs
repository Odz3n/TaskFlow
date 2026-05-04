
using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Tasks.Commands;

public record DeleteTaskCommand(
    Guid InitiatorId,
    Guid ProjectId,
    Guid TaskId
): ICommand<TaskDeleteDto>;
