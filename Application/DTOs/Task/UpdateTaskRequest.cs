using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs.Task;

public record UpdateTaskRequest(
    string Title,
    string Description,
    Guid AssigneeMemberId,
    Status Status,
    Priority Priority,
    DateTime DueDate
);