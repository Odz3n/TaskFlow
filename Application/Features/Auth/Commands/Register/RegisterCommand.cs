using TaskFlow.Application.DTOs.Auth;
using TaskFlow.Application.Interfaces.Messaging;

namespace TaskFlow.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword,
    IFormFile? Avatar
) : ICommand<RegisterResponse>;