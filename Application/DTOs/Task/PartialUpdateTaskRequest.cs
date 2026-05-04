using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs.Task;

public record PartialUpdateTaskRequest(
    Status Status
);