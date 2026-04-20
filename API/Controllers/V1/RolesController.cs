using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.Features.Roles.Commands;

namespace TaskFlow.API.Controllers.V1;

[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class RolesController : ApiController
{
    private readonly ISender _sender;
    public RolesController(ISender sender) 
        : base(sender)
    {
        _sender = sender;
    }
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
