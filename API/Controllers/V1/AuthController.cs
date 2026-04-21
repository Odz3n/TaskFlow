using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.DTOs.Auth;
using TaskFlow.Application.Features.Auth.Commands;
using TaskFlow.Application.Features.Auth.Commands.Register;

namespace TaskFlow.API.Controllers.V1;

[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public class AuthController : ApiController
{
    private readonly ISender _sender;
    public AuthController(ISender sender)
        : base(sender)
    {
        _sender = sender;
    }
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterUserRequest request,
        CancellationToken token
    )
    {
        var command = new RegisterCommand(
            FirstName: request.FirstName,
            LastName: request.LastName,
            UserName: request.UserName,
            Email: request.Email,
            Password: request.Password,
            ConfirmPassword: request.ConfirmPassword,
            AvatarUrl: request.AvatarUrl
        );

        var result = await _sender.Send(command, token);
        if (result.IsFailure)
            return HandleFailure(result);
            
        return Ok(result.Data);
    }
}