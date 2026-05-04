using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Tasks.Commands;

public record UpdateTaskCommand(
    Guid ProjectId,
    Guid TaskId,
    Guid InitiatorId,
    string Title,
    string Description,
    Guid AssigneeMemberId,
    Status Status,
    Priority Priority,
    DateTime DueDate
): ICommand<TaskUpdateResponse>;