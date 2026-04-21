using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Projects.Commands;

public record CreateProjectCommand(
    string Name,
    string? Description = null,
    List<Guid>? MemberIds = null) : ICommand;