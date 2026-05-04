using TaskFlow.Application.DTOs.Task;
using TaskFlow.Application.Interfaces.Messaging;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Features.Tasks.Commands;

public record CreateTaskCommand(
    string Title,
    string? Description,
    Guid ProjectId,
    Guid AssigneeMemberId,
    Guid InitiatorId,
    Priority Priority,
    DateTime DueDate
) : ICommand<TaskDto>;