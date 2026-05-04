using TaskFlow.Application.DTOs.Member;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Roles.Commands;

public record CreateRoleCommand(
    string Name
): ICommand<CreateRoleResponse>;