using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Projects.Commands;

public record CreateProjectCommand(
    Guid? InitiatorId,
    List<string> InitiatorRoles,
    string Name,
    string? Description = null,
    List<Guid>? MemberIds = null) : ICommand;