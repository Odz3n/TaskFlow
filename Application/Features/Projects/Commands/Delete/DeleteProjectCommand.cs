using TaskFlow.Application.Common;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Projects.Commands;

public record DeleteProjectCommand(
    Guid Id,
    Guid InitiatorId
) : ICommand<bool>;
