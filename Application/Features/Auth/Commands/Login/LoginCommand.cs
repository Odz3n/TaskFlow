using TaskFlow.Application.DTOs.Auth;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
): ICommand<LoginResponse>;