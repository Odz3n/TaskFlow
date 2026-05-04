using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Abstractions;
using TaskFlow.Application.Features.Users.Queries;
using TaskFlow.Domain.Models;

namespace MyApp.Namespace;

/// <summary>
/// Controller responsible for managing users.
/// </summary>
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public class UsersController : ApiController
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersController"/> class.
    /// </summary>
    /// <param name="sender">Mediator sender instance for handling requests.</param>
    public UsersController(ISender sender)
        : base(sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Retrieves a list of users based on the specified query parameters.
    /// </summary>
    /// <param name="query">Query parameters for filtering and retrieving users.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>
    /// Returns a list of <see cref="User"/> if the request is successful; otherwise, returns an error.
    /// </returns>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpGet]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers(
        [FromQuery] GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
    }
}