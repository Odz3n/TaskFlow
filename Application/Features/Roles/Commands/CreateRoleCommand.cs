using TaskFlow.Application.DTOs.Responses.Roles;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Roles.Commands;

public record CreateRoleCommand(
    string Name
): ICommand<CreateRoleResponse>;