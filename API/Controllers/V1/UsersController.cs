using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Features.Users;
using TaskFlow.Domain.Models;

namespace MyApp.Namespace
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;
        public UsersController(
            IMediator mediator,
            UserManager<User> userManager
        )
        {
            _mediator = mediator;
            _userManager = userManager;
        }
        [HttpGet]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(
            [FromForm] GetUsersQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }
    }
}
