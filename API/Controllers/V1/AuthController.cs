using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.DTOs.Auth;
using TaskFlow.Application.Features.Auth.Commands;
using TaskFlow.Application.Features.Auth.Commands.Login;
using TaskFlow.Application.Features.Auth.Commands.Register;

namespace TaskFlow.API.Controllers.V1;

/// <summary>
/// Provides authentication and user account management endpoints.
/// </summary>
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public class AuthController : ApiController
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="sender">Mediator sender used to dispatch commands.</param>
    public AuthController(ISender sender)
        : base(sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">The registration data including user credentials and optional avatar.</param>
    /// <param name="token">Token to cancel the request.</param>
    /// <returns>The registered user information.</returns>
    /// <response code="200">User registered successfully.</response>
    /// <response code="400">Invalid registration data.</response>
    [HttpPost("register")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> RegisterAsync(
        [FromForm] RegisterUserRequest request,
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
            Avatar: request.Avatar
        );

        var result = await _sender.Send(command, token);
        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Data);
    }

    /// <summary>
    /// Authenticates a user and returns an access token.
    /// </summary>
    /// <param name="command">Login credentials.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>Authentication result containing access token and user info.</returns>
    /// <response code="200">Authentication successful.</response>
    /// <response code="400">Invalid credentials.</response>
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Data);
    }
}