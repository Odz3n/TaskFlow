using TaskFlow.Application.DTOs.Responses;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Auth.Commands;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? AvatarUrl
) : ICommand<RegisterResponse>;