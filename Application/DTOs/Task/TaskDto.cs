using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs.Task;

public record TaskDto(
    Guid? id,
    string Title,
    Status Status,
    DateTime DueDate
)
{
    public bool IsOverdue => DueDate < DateTime.UtcNow;
};