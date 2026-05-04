using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Tasks.Commands;

public record PartialUpdateTaskCommand(
    Guid ProjectId,
    Guid TaskId,
    Guid InitiatorId,
    Status Status
): ICommand<TaskUpdateResponse>;