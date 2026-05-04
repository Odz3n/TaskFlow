using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.Features.Roles.Commands;

namespace TaskFlow.API.Controllers.V1;

/// <summary>
/// Provides endpoints for managing roles.
/// </summary>
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public class RolesController : ApiController
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="RolesController"/> class.
    /// </summary>
    /// <param name="sender">Mediator sender used to dispatch commands.</param>
    public RolesController(ISender sender)
        : base(sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="command">The role creation command containing role data.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The created role information.</returns>
    /// <response code="200">Role created successfully.</response>
    /// <response code="400">Invalid role data.</response>
    [HttpPost]
    public async Task<IActionResult> CreateRole(
        [FromBody] CreateRoleCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await _sender.Send(command, cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Data);
    }
}